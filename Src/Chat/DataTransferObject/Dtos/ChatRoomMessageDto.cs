using CommonShareDomain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DataTransferObject.Dtos
{

    public class ChatRoomMessageDto : ICloneable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public UserDto User { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatRoomMessageDto Clone()
        {
            return ((ICloneable)this).Clone() as ChatRoomMessageDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
