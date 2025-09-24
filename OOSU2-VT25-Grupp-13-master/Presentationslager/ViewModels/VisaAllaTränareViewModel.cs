using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Models; 
using Presentationslager.Personal.Tränarhantering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class VisaAllaTränareViewModel : ObservableObject
    {
        private readonly TränareController _tränareController;
        public Action? CloseAction { get; set; }

        // ObservableProperty för att hantera specialiseringar och vald specialisering
        [ObservableProperty]
        private ObservableCollection<string> specialiseringar = new() { "Alla", "Paddel", "Tennis", "Pingis", "Squash", "Badminton", "Innebandy" };

        [ObservableProperty]
        private string valdSpecialisering;

        // Här använder vi TränareModel istället för Tränare
        [ObservableProperty]
        private ObservableCollection<TränareModel> tränare = new();

        public VisaAllaTränareViewModel()
        {
            _tränareController = new TränareController();

            // Sätt standardval och ladda tränare
            ValdSpecialisering = "Alla";
            LaddaTränare();

            // Lyssna på ändringar av 'ValdSpecialisering' och ladda tränare igen
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ValdSpecialisering))
                {
                    LaddaTränare();
                }
            };
        }

        private void LaddaTränare()
        {
            // Hämta alla tränare från controller
            var allaTränare = _tränareController.HämtaAllaTränare().ToList();

            // Konvertera entiteterna till TränareModel
            var tränareModelList = allaTränare.Select(t => new TränareModel
            {
                TränareID = t.TränareID,
                Namn = t.Namn,
                Specialisering = t.Specialisering,
                Lösenord = t.Lösenord 
            }).ToList();

            // Filtrera baserat på vald specialisering
            if (ValdSpecialisering == "Alla")
            {
                Tränare = new ObservableCollection<TränareModel>(tränareModelList);
            }
            else
            {
                Tränare = new ObservableCollection<TränareModel>(tränareModelList
                    .Where(t => t.Specialisering != null && t.Specialisering.Equals(ValdSpecialisering, StringComparison.OrdinalIgnoreCase)));
            }
        }

        [RelayCommand]
        private void Tillbaka()
        {
            var fönster = new TränarhanteringWindow();
            fönster.Show();
            CloseAction?.Invoke();
        }
    }
}
