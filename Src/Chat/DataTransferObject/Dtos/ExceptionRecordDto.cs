using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DataTransferObject.Dtos
{

    public class ExceptionRecordDto : ICloneable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }

        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string OSType { get; set; }
        public string OSVersion { get; set; }
        public string Message { get; set; }
        public string CallStack { get; set; }
        public DateTime ExceptionTime { get; set; }

        public virtual UserDto User { get; set; }

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public ExceptionRecordDto Clone()
        {
            return ((ICloneable)this).Clone() as ExceptionRecordDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
