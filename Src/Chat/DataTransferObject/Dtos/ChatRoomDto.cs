using CommonShareDomain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DataTransferObject.Dtos
{

    public class ChatRoomDto : ICloneable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public RoomTypeEnum RoomType { get; set; } = RoomTypeEnum.PRIVATE;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatRoomDto Clone()
        {
            return ((ICloneable)this).Clone() as ChatRoomDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
