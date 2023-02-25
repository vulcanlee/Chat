using DomainData.Models;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> logger;
    #region 欄位與屬性
    #endregion

    #region 建構式
    public UserService(ILogger<UserService> logger)
    {
        this.logger = logger;
    }
    #endregion

    #region 其他服務方法
    public async Task<User> GetAsync(int id)
    {
        List<User> users = User.GetMyUsers();
        var checkUser = users.FirstOrDefault(x =>
        x.Id == id);
        await Task.Yield();
        if (checkUser == null)
        {
            return new User();
        }
        else
        {
            return checkUser;
        }

    }
    public async Task<(User, string)>
        CheckUserAsync(string account, string password)
    {
        List<User> users = User.GetMyUsers();
        var checkUser = users.FirstOrDefault(x =>
        x.Account.ToLower() == account.ToLower() &&
        x.Password == password);

        await Task.Yield();

        if (checkUser != null)
        {

        }
        else
        {
            return (null, "帳號或密碼不正確");
        }
        return (checkUser, "");
    }

    #endregion
}
