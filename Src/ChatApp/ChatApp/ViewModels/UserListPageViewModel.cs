using AutoMapper;
using Business.Events;
using Business.Services;
using ChatApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModels;

public partial class UserListPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IPageDialogService dialogService;
    private readonly IMapper mapper;
    private readonly UserService userService;
    private readonly IEventAggregator eventAggregator;
    #endregion

    #region Property Member
    [ObservableProperty]
    bool isRefreshing = false;
    [ObservableProperty]
    public ObservableCollection<UserModel> users =
        new ObservableCollection<UserModel>();
    #endregion

    #region Constructor
    public UserListPageViewModel(INavigationService navigationService,
        IPageDialogService dialogService, IMapper mapper,
        UserService userService, IEventAggregator eventAggregator)
    {
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        this.mapper = mapper;
        this.userService = userService;
        this.eventAggregator = eventAggregator;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    async Task TapRefreshAsync()
    {
        await RefreshAsync();
    }

    [RelayCommand]
    async Task TapUserAsync(UserModel user)
    {
        await Task.Yield();
        eventAggregator.GetEvent<ChatMemberAddRemoveEvent>().Publish(new ChatMemberAddRemovePayload()
        {
            ChatMemberChangeAction = ChatMemberChangeActionEnum.Add,
            User = user,
        });
    }

    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        if (parameters.GetNavigationMode() == Prism.Navigation.NavigationMode.New)
        {
            await RefreshAsync();
        }
    }
    #endregion

    #region Other Method
    async Task RefreshAsync()
    {
        IsRefreshing = true;
        var apiResult = await userService.GetAsync();
        if (apiResult.Status == false)
        {
            await dialogService.DisplayAlertAsync("錯誤",
                apiResult.Message, "確定");
            IsRefreshing = false;
            return;
        }

        var userModels = mapper.Map<List<UserModel>>(userService.Items);
        Users.Clear();
        Users = new ObservableCollection<UserModel>(userModels);
        IsRefreshing = false;
    }
    #endregion
    #endregion
}
