using AutoMapper;
using CommonDomainLayer.Magics;
using DomainData.Models;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class DatabaseInitService
    {
        private readonly ChatDBContext context;

        public IMapper Mapper { get; }
        public IConfiguration Configuration { get; }
        public ILogger<DatabaseInitService> Logger { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }

        public DatabaseInitService(ChatDBContext context, IMapper mapper,
            IConfiguration configuration, ILogger<DatabaseInitService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            Mapper = mapper;
            Configuration = configuration;
            Logger = logger;
            HttpContextAccessor = httpContextAccessor;
        }

        public async Task InitDBAsync(Action<string> OnUpdateMessage)
        {
            #region 適用於 Code First ，刪除資料庫與移除資料庫
            string Msg = "";
            Msg = $"適用於 Code First ，刪除資料庫與移除資料庫";
            OnUpdateMessage(Msg);
            Logger.LogInformation($"{Msg}");
            await context.Database.EnsureDeletedAsync();
            Msg = $"刪除資料庫";
            OnUpdateMessage(Msg);
            Logger.LogInformation($"{Msg}");
            await context.Database.EnsureCreatedAsync();
            Msg = $"建立資料庫";
            OnUpdateMessage(Msg);
            #endregion

            #region 還原預設紀錄
            DateTime currentNow;
            #region AccountPolicy
            currentNow = DateTime.Now;
            Msg = $"建立 AccountPolicy";
            OnUpdateMessage(Msg);
            #endregion

            #region 建立使用者紀錄
            await 建立使用者紀錄Async("開發模式");
            Msg = $"建立使用者紀錄";
            Logger.LogInformation($"{Msg}");
            #endregion

            #endregion
        }

        public async Task InitDataAsync(string InitializationMode, Action<string> OnUpdateMessage)
        {
            Random random = new Random();
            DateTime currentNow = DateTime.Now;

            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));

            #region 適用於 Code First ，刪除資料庫與移除資料庫
            string Msg = "";
            Msg = $"適用於 Code First ，刪除資料庫與移除資料庫";
            OnUpdateMessage(Msg);
            Logger.LogInformation($"{Msg}");
            try
            {
                currentNow = DateTime.Now;
                Msg = $"刪除資料庫";
                OnUpdateMessage($"{Msg}");
                await context.Database.EnsureDeletedAsync();
                OnUpdateMessage($"{Msg} ({DateTime.Now - currentNow})");
                Logger.LogInformation($"{Msg} ({DateTime.Now - currentNow})");
            }
            catch (Exception ex)
            {
                OnUpdateMessage($"{Msg} 發生例外異常 ({DateTime.Now - currentNow})");
                OnUpdateMessage(ex.Message);
                Logger.LogError(Msg, ex);
                return;
            }
            try
            {
                currentNow = DateTime.Now;
                Msg = $"建立資料庫";
                OnUpdateMessage($"{Msg}");
                await context.Database.EnsureCreatedAsync();
                OnUpdateMessage($"{Msg} ({DateTime.Now - currentNow})");
                Logger.LogInformation($"{Msg} ({DateTime.Now - currentNow})");
            }
            catch (Exception ex)
            {
                OnUpdateMessage($"{Msg} 發生例外異常 ({DateTime.Now - currentNow})");
                OnUpdateMessage(ex.Message);
                Logger.LogError(Msg, ex);
                return;
            }
            Logger.LogInformation($"{Msg}");
            #endregion

            #region 建立開發環境要用到的測試紀錄
            #region 建立使用者紀錄
            await 建立使用者紀錄Async(InitializationMode);
            Msg = $"建立使用者紀錄";
            Logger.LogInformation($"{Msg}");
            #endregion

            if (InitializationMode == "開發模式")
            {
            }
            #endregion

            Msg = $"資料庫初始化作業完成";
            OnUpdateMessage(Msg);
        }

        #region 建立相關紀錄
        private async Task 建立使用者紀錄Async(string InitializationMode)
        {
            #region 建立使用者紀錄 

            CleanTrackingHelper.Clean<User>(context);

            #region 建立 開發者
            var user = new User()
            {
                Account = MagicObject.開發者帳號,
                Name = $"開發者",
                Status = true,
                Salt = Guid.NewGuid().ToString(),
                CreateAt= DateTime.Now,
            };

            user.Salt = Guid.NewGuid().ToString();
            user.Password =
             PasswordHelper
             .GetPasswordSHA(user.Salt + MagicObject.PasswordSaltPostfix, "123");

            context.Add(user);
            await context.SaveChangesAsync();

            CleanTrackingHelper.Clean<User>(context);
            #endregion

            #region 建立 系統管理員
            var adminMyUser = new User()
            {
                Account = $"{MagicObject.系統管理員帳號}",
                Name = $"系統管理員 {MagicObject.系統管理員帳號}",
                Status = true,
                Salt = Guid.NewGuid().ToString(),
                CreateAt= DateTime.Now,
            };
            var adminRawPassword = "123";
            adminMyUser.Password =
                PasswordHelper.GetPasswordSHA(adminMyUser.Salt, adminRawPassword);

            context.Add(adminMyUser);
            await context.SaveChangesAsync();

            CleanTrackingHelper.Clean<User>(context);
            #endregion

            if (InitializationMode == "開發模式")
            {
                #region 建立 使用者
                int photoIndex = 2;
                foreach (var item in MagicObject.使用者帳號)
                {
                    var itemMyUser = await context.User
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Name == item);
                    if (itemMyUser == null)
                    {
                        string phtoFilename = $"emoji{photoIndex}.png";
                        photoIndex++;
                        if (photoIndex > 10) photoIndex = 2;
                        itemMyUser = new User()
                        {
                            Account = $"{item}",
                            Name = $"使用者 {item}",
                            Status = true,
                            Salt = Guid.NewGuid().ToString(),
                            CreateAt= DateTime.Now,
                        };
                        var userRawPassword = "123";
                        itemMyUser.Password =
                            PasswordHelper.GetPasswordSHA(itemMyUser.Salt, userRawPassword);

                        context.Add(itemMyUser);
                        await context.SaveChangesAsync();

                        CleanTrackingHelper.Clean<User>(context);
                    }
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
