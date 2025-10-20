using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresentationsLager.Models;
using PresentationsLager.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class BokningsHanteringWindowViewModel : ObservableObject
    {
        private readonly BokningsController _bokningsController;
        private readonly RestaurangController _restaurangController;

        [ObservableProperty]
        private AnvandareModel? inloggadAnvandare;

        [ObservableProperty]
        private DateTime valtDatum = DateTime.Today;

        [ObservableProperty]
        private TimeSpan valdTid = new TimeSpan(18, 0, 0);

        [ObservableProperty]
        private int antalGäster = 2;

        [ObservableProperty]
        private ObservableCollection<string> tillgangligaTider = new();

        [ObservableProperty]
        private ObservableCollection<AffärsLager.Controllers.BordMedStatus> bordMedStatus = new();

        public Action? CloseAction { get; set; }

        public BokningsHanteringWindowViewModel()
        {
            _bokningsController = new BokningsController();
            _restaurangController = new RestaurangController();

            InitializeTillgangligaTider();
        }

        public void Initialize(AnvandareModel anvandare)
        {
            InloggadAnvandare = anvandare;
            UppdateraVy();
        }

        private void InitializeTillgangligaTider()
        {
            var tider = _bokningsController.HamtaTillgangligaTider();
            TillgangligaTider.Clear();
            foreach (var tid in tider)
            {
                TillgangligaTider.Add(tid.ToString(@"hh\:mm"));
            }
        }

        [RelayCommand]
        private void UppdateraVy()
        {
            try
            {
                if (InloggadAnvandare?.HemmarestaurangID == null)
                {
                    MessageBox.Show("Ingen hemmarestaurang angiven för användaren", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Använd en ny BokningsController för att alltid få färsk data från databasen
                var freshBokningsController = new BokningsController();
                var bordStatus = freshBokningsController.HamtaAllaBordMedStatus(
                    InloggadAnvandare.HemmarestaurangID.Value,
                    ValtDatum,
                    ValdTid,
                    AntalGäster);

                BordMedStatus.Clear();
                foreach (var bord in bordStatus.OrderBy(b => b.Bordkod))
                {
                    BordMedStatus.Add(bord);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid uppdatering av bordöversikt: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void BordClick(AffärsLager.Controllers.BordMedStatus bordStatus)
        {
            try
            {
                if (bordStatus.Bokning != null)
                {
                    // Konvertera Bokning entity till BokningModel
                    var bokningModel = BokningModel.FromEntity(bordStatus.Bokning);
                    
                    var detaljWindow = new BokningsDetaljerWindow(bokningModel, InloggadAnvandare!);
                    var result = detaljWindow.ShowDialog();

                    if (result == true)
                    {
                        UppdateraVy();
                    }
                }
                else
                {
                    MessageBox.Show($"Bord {bordStatus.Bordkod} är ledigt för vald tid.", "Bordinfo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid visning av borddetaljer: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void MinskaGäster()
        {
            if (AntalGäster > 1)
            {
                AntalGäster--;
                UppdateraVy();
            }
        }

        [RelayCommand]
        private void ÖkaGäster()
        {
            if (AntalGäster < 12)
            {
                AntalGäster++;
                UppdateraVy();
            }
        }

        [RelayCommand]
        private void Stäng()
        {
            CloseAction?.Invoke();
        }

        partial void OnValtDatumChanged(DateTime value)
        {
            UppdateraVy();
        }

        partial void OnValdTidChanged(TimeSpan value)
        {
            UppdateraVy();
        }
    }
}