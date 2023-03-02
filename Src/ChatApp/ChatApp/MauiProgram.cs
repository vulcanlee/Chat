using Prism.Ioc;
using ChatApp.Helpers;
using ChatApp.ViewModels;
using ChatApp.Views;
using Business.Services;
using Business.DataModel;
using CommonLibrary.Helpers.Magics;

namespace ChatApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        #region AutoMapper 服務註冊
        builder.Services.AddAutoMapper(c => c.AddProfile<AutoMapping>());
        #endregion

        builder
            .UseMauiApp<App>()
            .UsePrism(prism =>
            {

                prism.RegisterTypes(container =>
                      {
                          #region View & ViewModel 對應註冊
                          container.RegisterForNavigation<MainPage, MainPageViewModel>();
                          container.RegisterForNavigation<NaviPage, NaviPageViewModel>();
                          container.RegisterForNavigation<FOPage, FOPageViewModel>();
                          container.RegisterForNavigation<SplashPage, SplashPageViewModel>();
                          container.RegisterForNavigation<HomePage, HomePageViewModel>();
                          container.RegisterForNavigation<HomePage, HomePageViewModel>();
                          container.RegisterForNavigation<OtpLoginPage, OtpLoginPageViewModel>();
                          container.RegisterForNavigation<OtpCodePage, OtpCodePageViewModel>();
                          container.RegisterForNavigation<LoginPage, LoginPageViewModel>();
                          container.RegisterForNavigation<UserLoginPage, UserLoginPageViewModel>();
                          container.RegisterForNavigation<ChatRoomPage, ChatRoomPageViewModel>();
                          container.RegisterForNavigation<ChatRoomDetailPage, ChatRoomDetailPageViewModel>();
                          container.RegisterForNavigation<AboutPage, AboutPageViewModel>();
                          container.RegisterForNavigation<AllUserPage, AllUserPageViewModel>();
                          #endregion

                          #region 客製服務註冊
                          container.RegisterSingleton<AppStatus>();
                          container.Register<OtpLoginService>();
                          container.Register<OtpVerifyCodeService>();
                          container.Register<UserLoginService>();
                          container.Register<UserService>();
                          container.Register<ChatRoomService>();
                          #endregion
                      })
                     .OnInitialized(() =>
                      {
                          // Do some initializations here
                      })
                     .OnAppStart(async navigationService =>
                     {
                         // Navigate to First page of this App
                         var result = await navigationService
                         .NavigateAsync("/SplashPage");
                         //.NavigateAsync("/FOPage/NaviPage/MainPage");
                         if (!result.Success)
                         {
                             System.Diagnostics.Debugger.Break();
                         }
                     });
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("materialdesignicons-webfont.ttf", MagicValue.FontName);
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
