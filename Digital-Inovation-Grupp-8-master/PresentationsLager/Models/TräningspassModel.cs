using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationsLager.Models
{
    public partial class TräningspassModel : ObservableObject
    {
        [ObservableProperty]
        private int träningspassID;

        [ObservableProperty]
        private int distans;

        [ObservableProperty]
        private string tempo;

        [ObservableProperty]
        private DateTime datum;

        [ObservableProperty]
        private string tid;

        [ObservableProperty]
        private string plats;

        [ObservableProperty]
        private string beskrivning;

        [ObservableProperty]
        private int? deltagarAntal = 0;

        [ObservableProperty]
        private int? maxDeltagare;

        [ObservableProperty]
        private ObservableCollection<MedlemTräningspassModel> medlemTräningspass = new();
    }
}
