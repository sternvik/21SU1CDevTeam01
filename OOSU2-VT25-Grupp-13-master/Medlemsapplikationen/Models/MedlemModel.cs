using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medlemsapplikationen.Models
{
    public partial class MedlemModel : ObservableObject
    {
        [ObservableProperty]
        private int medlemID;

        [ObservableProperty]
        private string namn;

        [ObservableProperty]
        private string telefonnummer;

        [ObservableProperty]
        private DateTime födelse;

        [ObservableProperty]
        private string epost;

        [ObservableProperty]
        private bool betalstatus;

        [ObservableProperty]
        private string lösenord;

        [ObservableProperty]
        private int poäng;

        [ObservableProperty]
        private int kalorier;

        [ObservableProperty]
        private ObservableCollection<MedlemTräningspassModel> medlemTräningspass = new();
    }
}
