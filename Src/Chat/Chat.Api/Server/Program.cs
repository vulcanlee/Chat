using Business.Helpers;
using BusinessHelpers;
using CommonDomainLayer.Configurations;
using CommonDomainLayer.Magics;
using DataTransferObject.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
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
                    //context.Response.ContentType = "application/json";
                    //context.Response.WriteAsync(JsonConvert.SerializeObject(apiResult)).Wait();
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    //if (!context.Request.Path.StartsWithSegments("/api") &&
                    //new HttpResponseMessage((HttpStatusCode)context.Response.StatusCode)
                    //.IsSuccessStatusCode) { 
                    //    context.HandleResponse(); //THIS solves my problem <----
                    //}
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    //Console.WriteLine("OnTokenValidated: " +
                    //    context.SecurityToken);
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

    #endregion

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();


    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");

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