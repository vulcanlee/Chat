using Business.DataModel;
using Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatApp.ViewModels;

public partial class OtpCodePageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IPageDialogService dialogService;
    private readonly AppStatus appStatus;
    private readonly OtpVerifyCodeService otpVerifyCodeService;
    private readonly OtpLoginService otpLoginService;
    private readonly UserService userService;
    #endregion

    #region Property Member
    [ObservableProperty]
    bool showStage1 = true;
    [ObservableProperty]
    bool showStage2 = false;
    [ObservableProperty]
    string phoneNumber = string.Empty;
    [ObservableProperty]
    string code = string.Empty;
    [ObservableProperty]
    string needCode = string.Empty;
    #endregion

    #region Constructor
    public OtpCodePageViewModel(INavigationService navigationService,
        IPageDialogService dialogService, AppStatus appStatus,
        OtpVerifyCodeService otpVerifyCodeService, OtpLoginService otpLoginService,
        UserService userService)
    {
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        this.appStatus = appStatus;
        this.otpVerifyCodeService = otpVerifyCodeService;
        this.otpLoginService = otpLoginService;
        this.userService = userService;

#if DEBUG
        phoneNumber = "0912345678";
#endif
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    async Task GetVerifyCode()
    {
        var apiResult = await otpVerifyCodeService.GetAsync(PhoneNumber);
        if (apiResult.Status == true)
        {
            NeedCode = otpVerifyCodeService.SingleItem.VerifyCode;
            ShowStage1 = false;
            ShowStage2 = true;
        }
        else
        {
            await dialogService.DisplayAlertAsync("錯誤", apiResult.Message, "確定");
        }
    }

    [RelayCommand]
    async Task SendVerifyCode()
    {
        var apiResult = await otpLoginService.GetAsync(PhoneNumber, Code);
        if (apiResult.Status == true)
        {
            await appStatus.FromLoginResponseDtoAsync(otpLoginService.SingleItem);
            apiResult = await userService.GetAsync(otpLoginService.SingleItem.Id);
            if (apiResult.Status == true)
            {
                appStatus.User = userService.SingleItem;
                await appStatus.WriteAsync();
                await navigationService.NavigateAsync("/FOPage/NaviPage/HomePage");
                return;
            }
            else
            {
                await dialogService.DisplayAlertAsync("錯誤", apiResult.Message, "確定");
            }
        }
        else
        {
            await dialogService.DisplayAlertAsync("錯誤", apiResult.Message, "確定");
        }
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
