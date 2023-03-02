using CommonShareDomain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Models;

public partial class ChatRoomModel : ObservableObject, ICloneable
{
    [ObservableProperty]
    public int id = 0;
    [ObservableProperty]
    public string name = string.Empty;
    [ObservableProperty]
    public RoomTypeEnum roomType  = RoomTypeEnum.PRIVATE;
    [ObservableProperty]
    public DateTime? createAt = default(DateTime);
    [ObservableProperty]
    public DateTime? updateAt = default(DateTime);

    #region 介面實作
    public ChatRoomModel Clone()
    {
        return ((ICloneable)this).Clone() as ChatRoomModel;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }
    #endregion
}
