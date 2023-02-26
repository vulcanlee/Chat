using Business.Helpers;
using DomainData.Models;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Api.Helpers
{
    public static class AddCustomServiceHelper
    {
        public static IServiceCollection AddProjetService(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IOtpService, OtpService>();
            services.AddTransient<IChatRoomService, ChatRoomService>();
            services.AddTransient<IChatRoomMemberService, ChatRoomMemberService>();
            services.AddTransient<IChatRoomMessageService, ChatRoomMessageService>();
            services.AddTransient<IExceptionRecordService, ExceptionRecordService>();
            services.AddTransient<JwtGenerateHelper>();
            services.AddTransient<DatabaseInitService>();
            return services;
        }
    }
}
