using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDomainLayer.Magics
{
    public class MagicObject
    {
        public const string SectionNameOfCustomNLogConfiguration = "CustomNLog";
        public const string SectionNameOfJwtConfiguration = "JWT";
        public const string SectionNameOfChatSystemAssistantConfiguration = "ChatSystemAssistant";
        public const string SectionNameOfConnectionStringConfiguration = "ConnectionStrings";
        public const string CookieAuthenticationScheme = "BackendCookieAuthenticationScheme"; // CookieAuthenticationDefaults.AuthenticationScheme
        public const string JwtBearerAuthenticationScheme = "BackendJwtBearerAuthenticationScheme"; // JwtBearerDefaults.AuthenticationScheme
        public const string ClaimTypeRoleNameSymbol = "role";
        public const string RoleRefreshToken = "RefreshToken";
        public const string RoleUser = "User";
        public const string RoleAdmin = "Admin";
        public const string DefaultConnectionString = "Admin";
        public const string DefaultSQLiteConnectionString = "Admin";

        #region 帳號相關
        public const string 開發者帳號 = "god";
        public const string 系統管理員帳號 = "admin";
        public static string[] 使用者帳號 = { "user1", "user2", "user3", "user4", "user5",
        "user6", "user6", "user7", "user8", "user9",
        "user10", "user11", "user12", "user13", "user14", "user15",
        "user16", "user16", "user17", "user18", "user19"};
        #endregion

        public const string HasException = "發生例外異常";
        public const string CannotFindRecordForUpdate = "無法發現要修改的紀錄";
    }
}
