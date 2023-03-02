using AutoMapper;
using Business.Services;
using ChatApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataTransferObject.Dtos;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModels;

public partial class ChatRoomPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IPageDialogService dialogService;
    private readonly IMapper mapper;
    private readonly ChatRoomService chatRoomService;
    #endregion

    #region Property Member
    [ObservableProperty]
    bool isRefreshing = false;
    [ObservableProperty]
    ObservableCollection<ChatRoomModel> chatRooms =
        new ObservableCollection<ChatRoomModel>();
    #endregion

    #region Constructor
    public ChatRoomPageViewModel(INavigationService navigationService,
        IPageDialogService dialogService, IMapper mapper,
        ChatRoomService chatRoomService)
    {
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        this.mapper = mapper;
        this.chatRoomService = chatRoomService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    async Task GetChatRoomAsync()
    {

    }

    [RelayCommand]
    async Task TapItemAsync(ChatRoomModel chatRoom)
    {

    }

    [RelayCommand]
    async Task AddNewChatRoomAsync()
    {
        await navigationService.NavigateAsync("ChatRoomDetailPage");
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
            #region 第一次顯示，從裝置上的記憶卡讀出之前的儲存紀錄
            await chatRoomService.ReadFromFileAsync();
            var allChartRooms = mapper.Map<List<ChatRoomModel>>(chatRoomService.Items);
            chatRooms = new ObservableCollection<ChatRoomModel>(allChartRooms);
            #endregion
        }
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
