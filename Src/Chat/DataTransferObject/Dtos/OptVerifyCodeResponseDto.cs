using System;
using System.ComponentModel;

namespace DataTransferObject.Dtos
{
    public class OptVerifyCodeResponseDto : ICloneable, INotifyPropertyChanged
    {
        public string VerifyCode { get; set; }

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public LoginResponseDto Clone()
        {
            return ((ICloneable)this).Clone() as LoginResponseDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
