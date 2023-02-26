using CommonShareDomain.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Dtos
{
    public class UserDto : ICloneable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Account { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Salt { get; set; }
        public string PhoneNumber { get; set; } = String.Empty;
        public bool Status { get; set; }
        public string PhotoFileName { get; set; } = "emoji1.png";
        public UserTypeEnum UserType { get; set; } = UserTypeEnum.LOCAL;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public UserDto Clone()
        {
            return ((ICloneable)this).Clone() as UserDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
