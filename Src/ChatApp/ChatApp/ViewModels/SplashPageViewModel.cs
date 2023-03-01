using Business.DataModel;
using CommonLibrary.Helpers.Magics;
using CommonLibrary.Helpers.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatApp.ViewModels;

public partial class SplashPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly AppStatus appStatus;
    #endregion

    #region Property Member
    [ObservableProperty]
    string message = string.Empty;
    [ObservableProperty]
    bool retryNetwork = false;
    #endregion

    #region Constructor
    public SplashPageViewModel(INavigationService navigationService,
        AppStatus appStatus)
    {
        this.navigationService = navigationService;
        this.appStatus = appStatus;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    async Task CheckNetwork()
    {
        RetryNetwork = false;

        await LaunchPrepare();
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        await LaunchPrepare();
    }
    #endregion

    #region Other Method
    async Task LaunchPrepare()
    {
        Message = $"請稍後，系統啟動中";
        await Task.Delay(3000);

        var isConnectNetwork = NetworkHelper.IsConnected();

        if (!isConnectNetwork)
        {
            RetryNetwork = true;
            return;
        }

        await appStatus.ReadAsync();

        var foo = appStatus.SystemStatus;
        if (appStatus.SystemStatus.IsLogin == true &&
            appStatus.SystemStatus.TokenExpireDatetime > DateTime.Now)
        {
            await navigationService.NavigateAsync("/FOPage/NaviPage/HomePage");
        }
        else
        {
            await navigationService.NavigateAsync("/NaviPage/LoginPage");
        }
    }
    #endregion
    #endregion
}
