using CommonShareDomain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using DataTransferObject.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Models;

public partial class ChatRoomMemberModel : ObservableObject, ICloneable
{
    [ObservableProperty]
    public int id = 0;
    [ObservableProperty]
    public int chatRoomId = 0;
    [ObservableProperty]
    public int userId = 0;
    [ObservableProperty]
    public UserModel user;
    [ObservableProperty]
    public string name = string.Empty;
    [ObservableProperty]
    public RoomTypeEnum roomType  = RoomTypeEnum.PRIVATE;
    [ObservableProperty]
    public DateTime? createAt = default(DateTime);
    [ObservableProperty]
    public DateTime? updateAt = default(DateTime);



    #region 介面實作
    public ChatRoomMemberModel Clone()
    {
        return ((ICloneable)this).Clone() as ChatRoomMemberModel;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }
    #endregion
}
