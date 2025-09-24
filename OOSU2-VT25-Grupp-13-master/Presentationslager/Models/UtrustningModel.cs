using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.Models
{
    public partial class UtrustningModel : ObservableObject
    {
        [ObservableProperty]
        private int utrustningID;

        [ObservableProperty]
        private string namn;

        [ObservableProperty]
        private string kategori;

        [ObservableProperty]
        private string skick;

        [ObservableProperty]
        private int tillgängliga;


        [ObservableProperty]
        private ObservableCollection<UtlåningModel> utlåningar;
    }
}
