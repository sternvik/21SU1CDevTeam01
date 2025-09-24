using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Personal.Utrustningshantering;
using System.Collections.ObjectModel;
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class RegistreraUtrustningViewModel : ObservableObject
    {
        private readonly UtrustningController _utrustningController;
        public Action? CloseAction { get; set; }

        // Egenskaper för bindning till UI-komponenter
        [ObservableProperty]
        private string namn;

        [ObservableProperty]
        private string kategori;

        [ObservableProperty]
        private string skick;

        [ObservableProperty]
        private int tillgängliga;

        public ObservableCollection<string> SkickList { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> KategoriList { get; } = new ObservableCollection<string>();

        // Konstruktor laddar data från controller
        public RegistreraUtrustningViewModel()
        {
            _utrustningController = new UtrustningController();
            LaddaSkick();
            LaddaKategori();
        }

        private void LaddaSkick()
        {
            var skick = _utrustningController.HämtaSkick();
            SkickList.Clear();
            foreach (var item in skick)
            {
                SkickList.Add(item);
            }
            Skick = SkickList.FirstOrDefault();
        }

        private void LaddaKategori()
        {
            var kategori = _utrustningController.HämtaKategorier();
            KategoriList.Clear();
            foreach (var item in kategori)
            {
                KategoriList.Add(item);
            }
            Kategori = KategoriList.FirstOrDefault();
        }

        [RelayCommand]
        private void RegistreraUtrustning()
        {
            if (string.IsNullOrWhiteSpace(Namn) || string.IsNullOrWhiteSpace(Kategori) || string.IsNullOrWhiteSpace(Skick))
            {
                MessageBox.Show("Alla fält måste fyllas i!");
                return;
            }

            if (Tillgängliga < 0)
            {
                MessageBox.Show("Ange ett giltigt antal (heltal, minst 0)!");
                return;
            }

            Utrustning utrustning = new Utrustning
            {
                Namn = Namn,
                Kategori = Kategori,
                Skick = Skick,
                Tillgängliga = Tillgängliga
            };

            _utrustningController.RegistreraUtrustning(utrustning);
            MessageBox.Show("Utrustning registrerad!");
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new UtrustningshanteringWindow().Show();
            CloseAction?.Invoke();
        }
    }
}
