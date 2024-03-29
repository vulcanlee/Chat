﻿using AutoMapper;
using Business.Events;
using Business.Services;
using ChatApp.Models;
using CommonLibrary.Helpers.Magics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataTransferObject.Dtos;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModels;

public partial class ChatRoomDetailPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IPageDialogService dialogService;
    private readonly IMapper mapper;
    private readonly ChatRoomService chatRoomService;
    private readonly IEventAggregator eventAggregator;
    string crudActionName = string.Empty;
    #endregion

    #region Property Member
    [ObservableProperty]
    ChatRoomModel currentChatRoom = new ChatRoomModel();
    [ObservableProperty]
    ObservableCollection<UserModel> users = new();
    #endregion

    #region Constructor
    public ChatRoomDetailPageViewModel(INavigationService navigationService,
        IPageDialogService dialogService, IMapper mapper,
        ChatRoomService chatRoomService, IEventAggregator eventAggregator)
    {
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        this.mapper = mapper;
        this.chatRoomService = chatRoomService;
        this.eventAggregator = eventAggregator;

        #region 訂閱使用者有異動事件
        SubscriptionToken token = eventAggregator.GetEvent<ChatMemberAddRemoveEvent>().Subscribe(payload =>
        {
            var aUser = users.FirstOrDefault(x => x.id == payload.User.id);
            if (aUser == null)
            {
                Users.Add(payload.User);
            }
        });
        #endregion
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(CurrentChatRoom.Name))
        {
            await dialogService.DisplayAlertAsync("操作錯誤",
                "聊天室名稱不可為空白", "確定");
            return;
        }

        CurrentChatRoom.CreateAt = DateTime.Now;
        CurrentChatRoom.UpdateAt = DateTime.Now;
        var chatRoomDto = mapper.Map<ChatRoomDto>(CurrentChatRoom);

        var apiResult = await chatRoomService.PostAsync(chatRoomDto);
    }

    [RelayCommand]
    async Task CalcelAsync()
    {
        await navigationService.GoBackAsync();
    }

    [RelayCommand]
    async Task JoinAsync()
    {
        await navigationService.NavigateAsync("UserListPage");
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
            crudActionName = parameters.GetValue<string>(MagicObject.CrudActionName);
            if (crudActionName == MagicObject.CrudAddAction)
            {
                CurrentChatRoom = new ChatRoomModel()
                {
                    Id = 0,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    Name = string.Empty,
                    RoomType = CommonShareDomain.Enums.RoomTypeEnum.PRIVATE,
                };
            }
            else if (crudActionName == MagicObject.CrudEditAction)
            {
                CurrentChatRoom = parameters.GetValue<ChatRoomModel>(MagicObject.ChatRoomObjectKeyName);
            }
        }
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
