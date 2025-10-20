using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PresentationsLager.ViewModels
{
    public partial class MenyWindowViewModel : ObservableObject
    {
        private readonly MenyController _menyController;
        private readonly RestaurangController _restaurangController;

        [ObservableProperty]
        private string restaurangNamn = string.Empty;

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> dagensLunch = new();

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> alaCarteMeny = new();

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> drycker = new();

        public Action? CloseAction { get; set; }

        public MenyWindowViewModel()
        {
            _menyController = new MenyController();
            _restaurangController = new RestaurangController();
        }

        public void Initialize(int restaurangId)
        {
            try
            {
                // Hämta restaurangnamn
                var restaurang = _restaurangController.HamtaRestaurangMedId(restaurangId);
                RestaurangNamn = restaurang?.Restaurangnamn ?? "Okänd restaurang";

                // Ladda meny för restaurangen
                var menyer = _menyController.HamtaMenyvarorForRestaurang(restaurangId);

                DagensLunch.Clear();
                AlaCarteMeny.Clear();
                Drycker.Clear();

                foreach (var meny in menyer)
                {
                    var menyItem = new MenyItemViewModel
                    {
                        MenyID = meny.MenyID,
                        Rattnamn = meny.Rattnamn,
                        Beskrivning = meny.Beskrivning ?? "",
                        Pris = meny.Pris,
                        Kategori = meny.Kategori
                    };

                    var kategoriLower = meny.Kategori.ToLower();
                    if (kategoriLower == "dagens lunch")
                    {
                        DagensLunch.Add(menyItem);
                    }
                    else if (kategoriLower == "à la carte" || kategoriLower == "a la carte")
                    {
                        AlaCarteMeny.Add(menyItem);
                    }
                    else if (kategoriLower.Contains("dryck"))
                    {
                        Drycker.Add(menyItem);
                    }
                    else
                    {
                        AlaCarteMeny.Add(menyItem);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Fel vid laddning av meny: {ex.Message}", "Fel",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Stang()
        {
            CloseAction?.Invoke();
        }
    }
}
