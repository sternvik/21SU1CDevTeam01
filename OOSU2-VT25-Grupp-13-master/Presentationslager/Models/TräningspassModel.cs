using CommunityToolkit.Mvvm.ComponentModel;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace Presentationslager.Models
{
    public partial class TräningspassModel : ObservableObject
    {
        [ObservableProperty]
        private int träningspassID;

        [ObservableProperty]
        private string aktivitet;

        [ObservableProperty]
        private DateTime datum;

        [ObservableProperty]
        private string tid;

        [ObservableProperty]
        private string plats;

        [ObservableProperty]
        private int tränareID;

        [ObservableProperty]
        private string beskrivning;

        [ObservableProperty]
        private int? deltagarAntal = 0;

        [ObservableProperty]
        private int? maxDeltagare;

        [ObservableProperty]
        private ObservableCollection<MedlemTräningspassModel> medlemTräningspass = new();

        [ObservableProperty]
        private TränareModel tränare;
    }
}
