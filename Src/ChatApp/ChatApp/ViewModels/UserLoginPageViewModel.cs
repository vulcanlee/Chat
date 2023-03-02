using Business.DataModel;
using Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatApp.ViewModels;

public partial class UserLoginPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IPageDialogService dialogService;
    private readonly AppStatus appStatus;
    private readonly UserLoginService userLoginService;
    #endregion

    #region Property Member
    [ObservableProperty]
    string account = string.Empty;
    [ObservableProperty]
    string password = string.Empty;
    [ObservableProperty]
    string message = string.Empty;
    #endregion

    #region Constructor
    public UserLoginPageViewModel(INavigationService navigationService,
        IPageDialogService dialogService,AppStatus appStatus,
        UserLoginService userLoginService)
    {
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        this.appStatus = appStatus;
        this.userLoginService = userLoginService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    async Task LoginAsync()
    {
        var apiResult = await userLoginService.PostAsync(new DataTransferObject.Dtos.LoginRequestDto()
        {
             Account = Account,
             Password = Password,
        });
        if (apiResult.Status == true)
        {
            await appStatus.FromLoginResponseDtoAsync(userLoginService.SingleItem);

            await navigationService.NavigateAsync("/FOPage/NaviPage/HomePage");
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
