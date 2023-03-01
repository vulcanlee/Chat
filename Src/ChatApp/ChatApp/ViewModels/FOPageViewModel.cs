using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChatApp.Helpers;
using ChatApp.Models;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModels;

public partial class FOPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    [ObservableProperty]
    ObservableCollection<MenuData> menuDatas = new ObservableCollection<MenuData>();
    #endregion

    #region Property Member
    #endregion

    #region Constructor Member
    public FOPageViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    async Task MenuTap(string command)
    {
        if (command == MagicValue.MenuHomeName)
        {
            await navigationService.NavigateAsync($"/FOPage/NaviPage/MainPage");
        }
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        if (parameters.GetNavigationMode() == Prism.Navigation.NavigationMode.New)
        {
            BuildMenuList();
        }
    }
    #endregion

    #region Other Method
    void BuildMenuList()
    {
        menuDatas.Clear();
        menuDatas.Add(new MenuData
        {
            Title = MagicValue.MenuHomeName,
            Icon = IconFont.Home,
        });
        menuDatas.Add(new MenuData
        {
            Title = MagicValue.MenuHomeName,
            Icon = IconFont.Account,
        });
        menuDatas.Add(new MenuData
        {
            Title = MagicValue.MenuHomeName,
            Icon = IconFont.ExitToApp,
        });
    }
    #endregion
    #endregion
}
