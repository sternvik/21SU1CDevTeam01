using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentationslager.Models;
using Presentationslager.Personal.Rapporter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.ViewModels
{
    public partial class UtrustningRapportViewModel : ObservableObject
    {
        private readonly UtrustningController _utrustningController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<UtrustningModel> trasigUtrustning = new();

        [ObservableProperty]
        private ObservableCollection<UtrustningModel> saknadUtrustning = new();

        public UtrustningRapportViewModel()
        {
            _utrustningController = new UtrustningController();
            LaddaRapport();
        }


        private void LaddaRapport()
        {
            var saknadutrustning = _utrustningController.HämtaSaknadUtrustning().Select(U => new UtrustningModel
            {
                UtrustningID = U.UtrustningID,
                Namn = U.Namn,
                Kategori = U.Kategori,
                Skick = U.Skick,
                Tillgängliga = U.Tillgängliga,
            }).ToList();

            SaknadUtrustning = new ObservableCollection<UtrustningModel>(saknadutrustning);

            var trasigutrustning = _utrustningController.HämtaTrasigUtrustning().Select(U => new UtrustningModel
            {
                UtrustningID = U.UtrustningID,
                Namn = U.Namn,
                Kategori = U.Kategori,
                Skick = U.Skick,
                Tillgängliga = U.Tillgängliga,
            }).ToList();

            TrasigUtrustning = new ObservableCollection<UtrustningModel>(trasigutrustning);
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new RapporterWindow().Show();
            CloseAction?.Invoke();
        }

    }
}
