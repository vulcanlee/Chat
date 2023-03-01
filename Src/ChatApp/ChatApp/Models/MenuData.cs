using CommunityToolkit.Mvvm.ComponentModel;
using ChatApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public partial class MenuData : ObservableObject
    {
        [ObservableProperty]
        string title = "";
        [ObservableProperty]
        string icon = "";
    }
}
