using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medlemsapplikationen.Models
{
    public partial class MedlemTräningspassModel : ObservableObject
    {
        [ObservableProperty]
        private int medlemID;

        [ObservableProperty]
        private int träningspassID;

        [ObservableProperty]
        private string status;

        [ObservableProperty]
        private MedlemModel medlem;

        [ObservableProperty]
        private TräningspassModel träningspass;
    }
}
