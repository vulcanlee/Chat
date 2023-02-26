using CommonShareDomain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DataTransferObject.Dtos
{

    public class ChatRoomMemberDto : ICloneable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public UserDto User { get; set; }
        public DateTime? CreateAt { get; set; }

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatRoomMemberDto Clone()
        {
            return ((ICloneable)this).Clone() as ChatRoomMemberDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
