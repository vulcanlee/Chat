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
    }
}
