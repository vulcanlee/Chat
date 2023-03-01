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
    string message=string.Empty;
    #endregion

    #region Constructor
    public SplashPageViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }
    #endregion

    #region Method Member
    #region Command Method
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        Message = $"請稍後，系統啟動中";
        await Task.Delay(3000);

        navigationService.NavigateAsync("/LoginPage");
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
