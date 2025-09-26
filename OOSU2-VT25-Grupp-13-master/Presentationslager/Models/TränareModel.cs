using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.Models
{
    public partial class TränareModel : ObservableObject
    {
        [ObservableProperty]
        private int tränareID;

        [ObservableProperty]
        private string namn;

        [ObservableProperty]
        private string specialisering;

        [ObservableProperty]
        private string lösenord;
    }
}
