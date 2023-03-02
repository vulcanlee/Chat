using CommonShareDomain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Models;

public class ChatRoomModel : ObservableObject, ICloneable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public RoomTypeEnum RoomType { get; set; } = RoomTypeEnum.PRIVATE;
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }

    #region 介面實作
    public event PropertyChangedEventHandler PropertyChanged;

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
