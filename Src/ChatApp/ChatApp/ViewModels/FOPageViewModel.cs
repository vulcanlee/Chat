using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChatApp.Helpers;
using ChatApp.Models;
using System.Collections.ObjectModel;
using CommonLibrary.Helpers.Magics;
using Business.DataModel;

namespace ChatApp.ViewModels;

public partial class FOPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IPageDialogService dialogService;
    private readonly AppStatus appStatus;
    [ObservableProperty]
    ObservableCollection<MenuData> menuDatas = new ObservableCollection<MenuData>();
    #endregion

    #region Property Member
    #endregion

    #region Constructor Member
    public FOPageViewModel(INavigationService navigationService,
        IPageDialogService dialogService,
        AppStatus appStatus)
    {
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        this.appStatus = appStatus;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    async Task MenuTap(string command)
    {
        if (command == MagicValue.MenuHomeName)
        {
            await navigationService.NavigateAsync($"/FOPage/NaviPage/HomePage");
        }
        else if (command == MagicValue.MenuLogoutName)
        {
            var result = await dialogService.DisplayAlertAsync("資訊",
                "確認要進行登出?", "確定", "取消");
            if (result == true)
            {
                await appStatus.LogoutAsync();
                await navigationService.NavigateAsync("/NaviPage/LoginPage");
            }
        }
        else if (command == MagicValue.MenuAboutName)
        {
            await navigationService.NavigateAsync($"/FOPage/NaviPage/AboutPage");
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
            Title = MagicValue.MenuAboutName,
            Icon = IconFont.Account,
        });
        menuDatas.Add(new MenuData
        {
            Title = MagicValue.MenuLogoutName,
            Icon = IconFont.ExitToApp,
        });
    }
    #endregion
    #endregion
}
