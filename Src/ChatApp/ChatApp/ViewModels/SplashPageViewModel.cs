using CommonLibrary.Helpers.Magics;
using CommonLibrary.Helpers.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatApp.ViewModels;

public partial class SplashPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    #endregion

    #region Property Member
    [ObservableProperty]
    string message = string.Empty;
    [ObservableProperty]
    bool retryNetwork = false;
    #endregion

    #region Constructor
    public SplashPageViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
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

        navigationService.NavigateAsync("/LoginPage");

    }
    #endregion
    #endregion
}
