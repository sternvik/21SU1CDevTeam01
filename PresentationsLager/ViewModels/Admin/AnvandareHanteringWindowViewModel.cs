using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using PresentationsLager.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PresentationsLager.ViewModels.Admin
{
    public partial class AnvandareHanteringWindowViewModel : ObservableObject
    {
        private readonly AnvandareController _anvandareController;
        private readonly RestaurangController _restaurangController;

        [ObservableProperty] private string sokNamn = string.Empty;
        [ObservableProperty] private string sokAnvandarnamn = string.Empty;
        [ObservableProperty] private string sokRoll = string.Empty;

        [ObservableProperty] private string nyAnvandarnamn = string.Empty;
        [ObservableProperty] private string nyLosenord = string.Empty;
        [ObservableProperty] private string bekraftaLosenord = string.Empty;
        [ObservableProperty] private string nyNamn = string.Empty;
        [ObservableProperty] private string nyRoll = string.Empty;
        [ObservableProperty] private bool nyAktiv = true;
        [ObservableProperty] private string valdAnvandareNyttLosenord = string.Empty;
        [ObservableProperty] private string valdAnvandareBekraftaLosenord = string.Empty;

        [ObservableProperty] private Restaurang? valdRestaurang;
        [ObservableProperty] private AnvandareModel? valdAnvandare;
        [ObservableProperty] private string statusMessage = string.Empty;

        [ObservableProperty] private Restaurang? nyValdRestaurang;
        [ObservableProperty] private string nyStatusMessage = string.Empty;

        [ObservableProperty] private ObservableCollection<AnvandareModel> hittadeAnvandare = new();
        [ObservableProperty] private ObservableCollection<Restaurang> restauranger = new();
        [ObservableProperty] private ObservableCollection<string> roller = new() { "Servitör", "Admin", "Restaurangchef", "VD" };

        private bool _isLoadingUser = false;
        public Action? CloseAction { get; set; }

        public AnvandareHanteringWindowViewModel()
        {
            _anvandareController = new AnvandareController();
            _restaurangController = new RestaurangController();

            LoadRestauranger();
        }

        private void LoadRestauranger()
        {
            try
            {
                restauranger.Clear();
                foreach (var r in _restaurangController.HamtaAllaRestauranger())
                    restauranger.Add(r);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av restauranger: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SokAnvandare()
        {
            try
            {
                StatusMessage = string.Empty;
                HittadeAnvandare.Clear();

                var resultat = _anvandareController.SokAnvandare(SokNamn, SokAnvandarnamn, SokRoll);
                foreach (var anv in resultat)
                    HittadeAnvandare.Add(AnvandareModel.FromEntity(anv));

                if (HittadeAnvandare.Count == 0)
                    StatusMessage = "Inga användare hittades.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid sökning: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SkapaAnvandare()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NyAnvandarnamn) ||
                    string.IsNullOrWhiteSpace(NyLosenord) ||
                    string.IsNullOrWhiteSpace(BekraftaLosenord) ||
                    string.IsNullOrWhiteSpace(NyNamn) ||
                    string.IsNullOrWhiteSpace(NyRoll))
                {
                    NyStatusMessage = "Alla fält markerade med * är obligatoriska.";
                    return;
                }

                if (NyLosenord != BekraftaLosenord)
                {
                    NyStatusMessage = "Lösenorden matchar inte.";
                    return;
                }

                var ny = new Anvandare
                {
                    Anvandarnamn = NyAnvandarnamn.Trim(),
                    Losenord = NyLosenord.Trim(),
                    Namn = NyNamn.Trim(),
                    Roll = NyRoll,
                    Aktiv = NyAktiv,
                    HemmarestaurangID = NyValdRestaurang?.RestaurangID
                };

                bool created = _anvandareController.SkapaAnvandare(ny);

                if (created)
                {
                    NyStatusMessage = $"Användare '{ny.Anvandarnamn}' skapad.";
                    NyAnvandarnamn = NyLosenord = BekraftaLosenord = NyNamn = NyRoll = string.Empty;
                    NyValdRestaurang = null;
                    NyAktiv = true;
                }
                else
                {
                    NyStatusMessage = "Kunde inte skapa användare (användarnamn kan redan finnas).";
                }
            }
            catch (Exception ex)
            {
                NyStatusMessage = $"Fel vid skapande: {ex.Message}";
            }
        }


        [RelayCommand]
        private void SparaLosenord()
        {
            if (ValdAnvandare == null) return;

            if (string.IsNullOrWhiteSpace(ValdAnvandareNyttLosenord) ||
                string.IsNullOrWhiteSpace(ValdAnvandareBekraftaLosenord))
            {
                StatusMessage = "Fyll i båda lösenordsfälten.";
                return;
            }

            if (ValdAnvandareNyttLosenord != ValdAnvandareBekraftaLosenord)
            {
                StatusMessage = "Lösenorden matchar inte.";
                return;
            }

            try
            {
                bool updated = _anvandareController.AndraLosenord(ValdAnvandare.AnvandarID, ValdAnvandareNyttLosenord);
                if (updated)
                {
                    StatusMessage = "Lösenordet uppdaterades.";
                    ValdAnvandareNyttLosenord = ValdAnvandareBekraftaLosenord = string.Empty;

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                        {
                            var nyPwdBox = window.FindName("ValdNyPasswordBox") as PasswordBox;
                            var bekräftaPwdBox = window.FindName("ValdBekraftaPasswordBox") as PasswordBox;
                            if (nyPwdBox != null) nyPwdBox.Password = string.Empty;
                            if (bekräftaPwdBox != null) bekräftaPwdBox.Password = string.Empty;
                        }
                    }
                }
                else StatusMessage = "Kunde inte uppdatera lösenordet.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid uppdatering av lösenord: {ex.Message}";
            }
        }

        partial void OnValdAnvandareChanged(AnvandareModel? anv)
        {
            if (anv == null) return;

            try
            {
                _isLoadingUser = true;

                if (anv.HemmarestaurangID.HasValue)
                {
                    var restaurang = Restauranger.FirstOrDefault(r => r.RestaurangID == anv.HemmarestaurangID);
                    ValdRestaurang = restaurang;
                    anv.Hemmarestaurang = restaurang;
                }
                else
                {
                    ValdRestaurang = null;
                    anv.Hemmarestaurang = null;
                }

                anv.PropertyChanged += ValdAnvandare_PropertyChanged;

                _isLoadingUser = false;
            }
            catch (Exception ex)
            {
                _isLoadingUser = false;
                StatusMessage = $"Fel vid laddning av användare: {ex.Message}";
            }
        }

        private void ValdAnvandare_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_isLoadingUser || ValdAnvandare == null) return;

            try
            {
                if (e.PropertyName == nameof(ValdAnvandare.Namn) ||
                    e.PropertyName == nameof(ValdAnvandare.Anvandarnamn) ||
                    e.PropertyName == nameof(ValdAnvandare.Roll) ||
                    e.PropertyName == nameof(ValdAnvandare.Hemmarestaurang))
                {
                    ValdAnvandare.HemmarestaurangID = ValdAnvandare.Hemmarestaurang?.RestaurangID;

                    _anvandareController.UppdateraAnvandare(ValdAnvandare.ToEntity());
                    StatusMessage = "Ändringar sparade.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid sparande: {ex.Message}";
            }
        }

        [RelayCommand]
        private void TaBortAnvandare()
        {
            if (ValdAnvandare == null)
            {
                StatusMessage = "Ingen användare vald för borttagning.";
                return;
            }

            var result = MessageBox.Show($"Vill du verkligen ta bort användaren '{ValdAnvandare.Anvandarnamn}'?",
                                         "Bekräfta borttagning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (_anvandareController.TaBortAnvandare(ValdAnvandare.AnvandarID))
                    {
                        StatusMessage = $"Användare '{ValdAnvandare.Anvandarnamn}' borttagen.";
                        HittadeAnvandare.Remove(ValdAnvandare);
                        ValdAnvandare = null;
                    }
                    else StatusMessage = "Kunde inte ta bort användare.";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Fel vid borttagning: {ex.Message}";
                }
            }
        }

        [RelayCommand]
        private void Tillbaka()
        {
            CloseAction?.Invoke();
        }
    }
}
