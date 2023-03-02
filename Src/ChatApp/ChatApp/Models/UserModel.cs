using CommonShareDomain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Models;

public partial class UserModel : ObservableObject, ICloneable
{
    [ObservableProperty]
    public int id = 0;
    [ObservableProperty]
    public string account = string.Empty;
    [ObservableProperty]
    public string password = string.Empty;
    [ObservableProperty]
    public string name = string.Empty;
    [ObservableProperty]
    public string salt = string.Empty;
    [ObservableProperty]
    public string phoneNumber = string.Empty;
    [ObservableProperty]
    public bool status = false;
    [ObservableProperty]
    public string photoFileName = string.Empty;
    [ObservableProperty]
    public UserTypeEnum userType = UserTypeEnum.LOCAL;
    [ObservableProperty]
    public DateTime? createAt = default(DateTime);
    [ObservableProperty]
    public DateTime? updateAt = default(DateTime);

    #region 介面實作
    public UserModel Clone()
    {
        return ((ICloneable)this).Clone() as UserModel;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }
    #endregion
}
