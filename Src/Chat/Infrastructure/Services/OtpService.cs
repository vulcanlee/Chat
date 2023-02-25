using DomainData.Models;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class OtpService : IOtpService
{
    private readonly ChatDBContext context;
    private readonly ILogger<OtpService> logger;
    #region 欄位與屬性
    #endregion

    #region 建構式
    public OtpService(ChatDBContext context, ILogger<OtpService> logger)
    {
        this.context = context;
        this.logger = logger;
    }
    #endregion

    #region 其他服務方法
    public async Task<string> GenerateVerifyCodeAsync(string phoneNumber)
    {
        var foo = await context.OtpVerifyHistory
            .AsNoTracking()
            .OrderByDescending(x => x.CreateAt)
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        if (foo != null)
        {
            var interval = DateTime.Now - foo.CreateAt;
            if (interval.Value.TotalMinutes == 0)
            {
                return "不可太過密集發送驗證碼，請稍後重新嘗試";
            }
        }

        Random random = new Random();
        string verifyCode = $"{random.Next(10000):D4}";

        OtpVerifyHistory otpVerifyHistory = new()
        {
            PhoneNumber = phoneNumber,
            CreateAt = DateTime.Now,
            Used = false,
            VerifyCode = verifyCode,
        };

        await context.OtpVerifyHistory.AddAsync(otpVerifyHistory);
        await context.SaveChangesAsync();

        CleanTrackingHelper.Clean<OtpVerifyHistory>(context);

        return verifyCode;
    }

    public async Task<User> LoginAsync(string phoneNumber)
    {
        var checkUser = context.User
            .AsNoTracking()
            .FirstOrDefault(x => x.PhoneNumber == phoneNumber);
        await Task.Yield();
        if (checkUser == null)
        {
            User user = new User()
            {
                Name = "尚未更新",
                PhoneNumber = phoneNumber,
                Account = Guid.NewGuid().ToString(),
                CreateAt = DateTime.Now,
                Salt = "",
                Status = true,
                PhotoFileName = "",
                UserType = CommonDomain.Enums.UserTypeEnum.OTP,
            };
            context.User.Add(user);
            await context.SaveChangesAsync();

            CleanTrackingHelper.Clean<OtpVerifyHistory>(context);

            return user;
        }
        else
        {
            return checkUser;
        }
    }

    public async Task<string> CheckVerifyCodeAsync(string phoneNumber, string verifyCode)
    {
        var otpVerifyHistory = await context.OtpVerifyHistory
            .AsNoTracking()
            .OrderByDescending(x => x.CreateAt)
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber && x.VerifyCode == verifyCode
            && x.Used == false);
        if (otpVerifyHistory == null)
        {
            return $"沒有發現 {phoneNumber} 可用驗證碼，請重新進行 OTP 驗證";
        }

        otpVerifyHistory.Used = true;
        context.OtpVerifyHistory.Update(otpVerifyHistory);
        await context.SaveChangesAsync();

        CleanTrackingHelper.Clean<OtpVerifyHistory>(context);

        return string.Empty;
    }
    #endregion
}
