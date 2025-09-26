using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.Models
{
    public partial class UtlåningModel : ObservableObject
    {
        [ObservableProperty]
        private int utlåningID;

        [ObservableProperty]
        private int medlemID;

        [ObservableProperty]
        private int utrustningID;

        [ObservableProperty]
        private DateTime utLåningsdatum;

        [ObservableProperty]
        private DateTime? återlämningsdatum;

        [ObservableProperty]
        private MedlemModel medlem;

        [ObservableProperty]
        private UtrustningModel utrustning;
    }
}
