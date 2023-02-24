using System;
using System.ComponentModel;

namespace DataTransferObject.Dtos
{
    public class MyUserDto : ICloneable, INotifyPropertyChanged
    {
        public string Account { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public MyUserDto Clone()
        {
            return ((ICloneable)this).Clone() as MyUserDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
