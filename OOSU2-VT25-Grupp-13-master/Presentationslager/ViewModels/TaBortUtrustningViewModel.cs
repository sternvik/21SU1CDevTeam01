using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Models;
using Presentationslager.Personal.Utrustningshantering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class TaBortUtrustningViewModel : ObservableObject
    {
        private readonly UtrustningController _utrustningController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<string> kategorier = new();

        [ObservableProperty]
        private ObservableCollection<UtrustningModel> utrustningLista = new();

        [ObservableProperty]
        private string valdKategori = "Alla";

        [ObservableProperty]
        private UtrustningModel valdUtrustning;


        public TaBortUtrustningViewModel()
        {
            _utrustningController = new UtrustningController();
            LaddaKategoriFilter();
            LaddaUtrustning();
        }

        partial void OnValdKategoriChanged(string value)
        {
            FiltreraUtrustning();
        }

        private void LaddaKategoriFilter()
        {
            var kategorier = _utrustningController.HämtaKategorier();
            kategorier.Insert(0, "Alla");
            Kategorier = new ObservableCollection<string>(kategorier);
        }

        private void LaddaUtrustning()
        {
            var hämtadutrustning = _utrustningController.HämtaAllUtrustning()
            .Select(u => new UtrustningModel
            {
                UtrustningID = u.UtrustningID,
                Namn = u.Namn,
                Kategori = u.Kategori,
                Skick = u.Skick,
                Tillgängliga = u.Tillgängliga,
            }).ToList();

            UtrustningLista = new ObservableCollection<UtrustningModel>(hämtadutrustning);

        }

        private void FiltreraUtrustning()
        {
            var allaUtrustning = _utrustningController.HämtaAllUtrustning()
                .Select(u => new UtrustningModel
                {
                    UtrustningID = u.UtrustningID,
                    Namn = u.Namn,
                    Kategori = u.Kategori,
                    Skick = u.Skick,
                    Tillgängliga = u.Tillgängliga,
                });

            if (!string.IsNullOrEmpty(ValdKategori) && ValdKategori != "Alla")
            {
                UtrustningLista = new ObservableCollection<UtrustningModel>(
                    allaUtrustning.Where(u => u.Kategori?.Equals(ValdKategori, StringComparison.OrdinalIgnoreCase) == true)
                );
            }
            else
            {
                UtrustningLista = new ObservableCollection<UtrustningModel>(allaUtrustning);
            }
        }

        [RelayCommand]
        private void TaBortUtrustning()
        {
            if (ValdUtrustning == null)
            {
                MessageBox.Show("Välj utrustning att ta bort!");
                return;
            }

            var utrustningEntity = new Utrustning
            {
                UtrustningID = ValdUtrustning.UtrustningID,
                Namn = ValdUtrustning.Namn,
                Kategori = ValdUtrustning.Kategori,
                Skick = ValdUtrustning.Skick,
                Tillgängliga = ValdUtrustning.Tillgängliga,
            };

            _utrustningController.TaBortUtrustning(utrustningEntity);
            MessageBox.Show("Utrustning borttagen!");
            LaddaUtrustning();
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new UtrustningshanteringWindow().Show();
            CloseAction?.Invoke();
        }
    }
}

