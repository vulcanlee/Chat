using Business.Helpers;
using BusinessHelpers;
using Chat.Api.Hubs;
using CommonDomainLayer.Configurations;
using CommonDomainLayer.Magics;
using DataTransferObject.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using System.Net;
using System.Text;


#region NLog ���Ұʫŧi
// NLog �]�w���� : https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
#endregion

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region �[�J�A�Ȩ�e���� Add services to the container.
    #region �M�׽d���w���إߪ�
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();
    #endregion

    #region Swagger �|�Ψ쪺�A��
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    #endregion

    #region ���U���M�׷|�Ψ쪺�Ȼs�A��
    builder.Services.AddTransient<IMyUserService, MyUserService>();
    builder.Services.AddTransient<JwtGenerateHelper>();
    #endregion

    #region �����ﶵ�Ҧ�
    builder.Services.Configure<CustomNLogConfiguration>(builder.Configuration
        .GetSection(MagicObject.SectionNameOfCustomNLogConfiguration));
    builder.Services.Configure<JwtConfiguration>(builder.Configuration
        .GetSection(MagicObject.SectionNameOfJwtConfiguration));
    #endregion

    #region �[�J�ϥ� Cookie & JWT �{�һݭn���ŧi
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

    #region �P���F��(Same Origin Policy)
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

    #region ���U SignalR �ݭn�Ψ쪺�A��
    builder.Services.AddSignalR();
    #endregion

    #endregion

    var app = builder.Build();

    #region �ŧi�޹D�P�����n��

    #region ��JWT ���ͪ��ҥ~���`�A�ন APIResult
    app.Use(async (context, next) =>
    {
        await next();

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

    #region �ŧi NLog �n�ϥΨ쪺�ܼƤ��e
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

    #region �}�o�Ҧ����]�w
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();

        #region �ҥ� Swagger �����n��
        app.UseSwagger();
        app.UseSwaggerUI();
        #endregion
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    #endregion

    #region �j��ϥ� HTTPS
    app.UseHttpsRedirection();
    #endregion

    #region �ϥ� CORS
    app.UseCors("MyCors");
    #endregion

    #region �q�κ޲z�����n��
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();
    #endregion

    #region ���w�n�ϥΨϥΪ̻{�Ҫ������n��
    app.UseCookiePolicy();
    app.UseAuthentication();
    #endregion

    #region ���w�ϥα��v�ˬd�������n��
    app.UseAuthorization();
    #endregion

    #region �N���I����s�W�ܤ����n��޽u

    #region �ŧi SignalR �n�Ψ쪺����
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