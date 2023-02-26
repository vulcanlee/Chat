using System;
using System.ComponentModel;

namespace DataTransferObject.Dtos
{
    public class UserDto : ICloneable, INotifyPropertyChanged
    {
        public string Account { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;

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
