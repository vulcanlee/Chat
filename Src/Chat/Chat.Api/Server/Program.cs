using Business.Helpers;
using BusinessHelpers;
using Chat.Api.Helpers;
using Chat.Api.Hubs;
using CommonDomain.Configurations;
using CommonDomainLayer.Configurations;
using CommonDomainLayer.Magics;
using DataTransferObject.Dtos;
using DomainData.Models;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using System.Net;
using System.Text;


#region NLog 的啟動宣告
// NLog 設定說明 : https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
#endregion

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region 加入服務到容器內 Add services to the container.
    #region 專案範本預先建立的
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();
    #endregion

    #region Swagger 會用到的服務
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    #endregion

    #region 註冊本專案會用到的客製服務
    builder.Services.AddProjetService();
    #endregion

    #region 相關選項模式
    builder.Services.Configure<CustomNLogConfiguration>(builder.Configuration
        .GetSection(MagicObject.SectionNameOfCustomNLogConfiguration));
    builder.Services.Configure<JwtConfiguration>(builder.Configuration
        .GetSection(MagicObject.SectionNameOfJwtConfiguration));
    builder.Services.Configure<ConnectionStringConfiguration>(builder.Configuration
        .GetSection(MagicObject.SectionNameOfConnectionStringConfiguration));
    builder.Services.Configure<ChatSystemAssistantConfiguration>(builder.Configuration
        .GetSection(MagicObject.SectionNameOfChatSystemAssistantConfiguration));
    #endregion

    #region EF Core & AutoMapper 使用的宣告
    ConnectionStringConfiguration connectionStringConfiguration =
        builder.Services.BuildServiceProvider()
        .GetRequiredService<IOptions<ConnectionStringConfiguration>>()
        .Value;
    ChatSystemAssistantConfiguration chatSystemAssistantConfiguration =
        builder.Services.BuildServiceProvider()
        .GetRequiredService<IOptions<ChatSystemAssistantConfiguration>>()
        .Value;

    if (chatSystemAssistantConfiguration.UseSQLite == false)
    {
        builder.Services.AddDbContext<ChatDBContext>(options =>
        options.UseSqlServer(connectionStringConfiguration.ChatDefaultConnection),
        ServiceLifetime.Transient);
    }
    else
    {
        builder.Services.AddDbContext<ChatDBContext>(options =>
        options.UseSqlite(connectionStringConfiguration.ChatSQLiteDefaultConnection),
        ServiceLifetime.Transient);
    }

    builder.Services.AddAutoMapper(c => c.AddProfile<AutoMapping>());
    #endregion

    #region 加入使用 Cookie & JWT 認證需要的宣告
    JwtConfiguration jwtConfiguration =
    builder.Services.BuildServiceProvider()
    .GetRequiredService<IOptions<JwtConfiguration>>()
    .Value;

    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
    });
    builder.Services.AddAuthentication(
        MagicObject.CookieAuthenticationScheme)
        .AddCookie(MagicObject.CookieAuthenticationScheme, options =>
        {
            //options.Events.OnRedirectToAccessDenied =
            //    options.Events.OnRedirectToLogin = c =>
            //    {
            //        c.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //        return Task.FromResult<object>(null);
            //    };
        })
        .AddJwtBearer(MagicObject.JwtBearerAuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidAudience = jwtConfiguration.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(jwtConfiguration.IssuerSigningKey)),
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromMinutes(jwtConfiguration.ClockSkew),
            };
            options.Events = new JwtBearerEvents()
            {
                OnAuthenticationFailed = context =>
                {
                    //context.Response.StatusCode = 401;
                    context.Response.HttpContext.Features
                    .Get<IHttpResponseFeature>().ReasonPhrase =
                    context.Exception.Message;

                    APIResult<object> apiResult = JWTTokenFailHelper
                    .GetFailResult<object>(context.Exception);
                    context.HttpContext.Items.Add("ExceptionJson", apiResult);
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    return Task.CompletedTask;
                }

            };
        });
    #endregion

    #region NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    #endregion

    #region 同源政策(Same Origin Policy)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("MyCors",
                              policy =>
                              {
                                  policy.AllowAnyOrigin()
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                              });
    });

    #endregion

    #region 註冊 SignalR 需要用到的服務
    builder.Services.AddSignalR();
    #endregion

    #region 工程除錯模式用服務
    builder.Services.AddSingleton<EngineerModeData>(new EngineerModeData());
    builder.Services.AddTransient<LoggerHelper>();
    builder.Services.AddSingleton<OSPerformanceLoggingHelper>();
    //builder.Services.AddTransient<RequestResponseLoggingMiddleware>();
    #endregion

    #endregion

    var app = builder.Build();

    #region 系統效能數據蒐集的準備工作
    var osPerformanceLoggingHelper = app.Services.GetService<OSPerformanceLoggingHelper>();
    await osPerformanceLoggingHelper.Prepare();
    #endregion

    #region 宣告管道與中介軟體

    #region 把JWT 產生的例外異常，轉成 APIResult
    app.Use(async (context, next) =>
    {
        await next();

        await Console.Out.WriteLineAsync($"{context.Request.GetDisplayUrl()}");
        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
        {
            if (context.Items.ContainsKey("ExceptionJson"))
            {
                var item = context.Items["ExceptionJson"];
                if (item is APIResult<object>)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.WriteAsync(JsonConvert.SerializeObject(item)).Wait();
                }
            }
        }
    });
    #endregion

    #region 宣告 NLog 要使用到的變數內容
    CustomNLogConfiguration optionsCustomNLogConfiguration =
        app.Services.GetRequiredService<IOptions<CustomNLogConfiguration>>()
        .Value;
    LogManager.Configuration.Variables["LogRootPath"] =
        optionsCustomNLogConfiguration.LogRootPath;
    LogManager.Configuration.Variables["AllLogMessagesFilename"] =
        optionsCustomNLogConfiguration.AllLogMessagesFilename;
    LogManager.Configuration.Variables["AllWebDetailsLogMessagesFilename"] =
        optionsCustomNLogConfiguration.AllWebDetailsLogMessagesFilename;
    #endregion

    #region Set X-FRAME-OPTIONS in ASP.NET Core
    // https://blog.johnwu.cc/article/asp-net-core-response-header.html
    // https://dotnetcoretutorials.com/2017/01/08/set-x-frame-options-asp-net-core/
    // https://developer.mozilla.org/zh-TW/docs/Web/HTTP/Headers/X-Frame-Options
    // https://blog.darkthread.net/blog/remove-iis-response-server-header/
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Remove("X-Frame-Options");
        context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
        await next();
    });
    #endregion

    #region 開發模式的設定
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();

        #region 啟用 Swagger 中介軟體
        app.UseSwagger();
        app.UseSwaggerUI();
        #endregion
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();

        #region 強制使用 HTTPS
        app.UseHttpsRedirection();
        #endregion
    }
    #endregion

    #region 嵌入 HTTP Request / Response 的 Logging
    app.UseRequestResponseLogging();
    #endregion

    #region 使用 CORS
    app.UseCors("MyCors");
    #endregion

    #region 通用管理中介軟體
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();
    #endregion

    #region 指定要使用使用者認證的中介軟體
    app.UseCookiePolicy();
    app.UseAuthentication();
    #endregion

    #region 指定使用授權檢查的中介軟體
    app.UseAuthorization();
    #endregion

    #region 將端點執行新增至中介軟體管線

    #region 宣告 SignalR 要用到的路由
    app.MapHub<MyChatHub>("/myChatHub");
    #endregion

    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");
    #endregion
    #endregion

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}