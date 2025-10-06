using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PresentationsLager.ViewModels.Admin
{
    public partial class KunderHanteringWindowViewModel : ObservableObject
    {
        private readonly KundController _kundController;
        private readonly RegionController _regionController;
        private readonly RestaurangController _restaurangController;
        private readonly ExtraController _extraController;

        [ObservableProperty]
        private string sokTelefon = string.Empty;

        [ObservableProperty]
        private string sokNamn = string.Empty;

        [ObservableProperty]
        private string sokEmail = string.Empty;

        [ObservableProperty]
        private string nyKundNamn = string.Empty;

        [ObservableProperty]
        private string nyKundTelefon = string.Empty;

        [ObservableProperty]
        private string nyKundEmail = string.Empty;

        [ObservableProperty]
        private Region? valdRegion;

        [ObservableProperty]
        private Restaurang? valdRestaurang;

        [ObservableProperty]
        private Kund? valdKund;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private int antalHittadeKunder = 0;

        [ObservableProperty]
        private ObservableCollection<Kund> hittadeKunder = new();

        [ObservableProperty]
        private ObservableCollection<Region> regioner = new();

        [ObservableProperty]
        private ObservableCollection<Restaurang> restauranger = new();

        public Action? CloseAction { get; set; }

        public KunderHanteringWindowViewModel()
        {
            _kundController = new KundController();
            _regionController = new RegionController();
            _restaurangController = new RestaurangController();
            _extraController = new ExtraController();

            LoadRegioner();
        }

        private void LoadRegioner()
        {
            try
            {
                Regioner.Clear();
                foreach (var region in _regionController.HamtaAllaRegioner())
                    Regioner.Add(region);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av regioner: {ex.Message}";
            }
        }

        partial void OnValdRegionChanged(Region? value)
        {
            if (value != null)
                LoadRestaurangerForRegion(value.RegionID);
            else
            {
                Restauranger.Clear();
                ValdRestaurang = null;
            }
        }

        private void LoadRestaurangerForRegion(int regionId)
        {
            try
            {
                Restauranger.Clear();
                foreach (var r in _restaurangController.HamtaRestaurangerForRegion(regionId))
                    Restauranger.Add(r);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av restauranger: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SokKund()
        {
            try
            {
                StatusMessage = string.Empty;
                if (string.IsNullOrWhiteSpace(SokTelefon) &&
                    string.IsNullOrWhiteSpace(SokNamn) &&
                    string.IsNullOrWhiteSpace(SokEmail))
                {
                    StatusMessage = "Ange minst ett sökkriterium";
                    return;
                }

                HittadeKunder.Clear();
                foreach (var kund in _kundController.SokKunder(SokTelefon, SokNamn, SokEmail))
                    HittadeKunder.Add(kund);

                AntalHittadeKunder = HittadeKunder.Count;
                if (AntalHittadeKunder == 0)
                    StatusMessage = "Inga kunder hittades med angivna sökkriterier";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid sökning: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SkapaKund()
        {
            try
            {
                StatusMessage = string.Empty;
                if (string.IsNullOrWhiteSpace(NyKundNamn) || string.IsNullOrWhiteSpace(NyKundTelefon))
                {
                    StatusMessage = "Namn och telefonnummer är obligatoriska";
                    return;
                }

                var nyKund = new Kund
                {
                    Namn = NyKundNamn.Trim(),
                    Telefon = NyKundTelefon.Trim(),
                    Email = string.IsNullOrWhiteSpace(NyKundEmail) ? null : NyKundEmail.Trim(),
                    HemmarestaurangID = ValdRestaurang?.RestaurangID,
                    LojalitetsPoang = 0,
                    LojalitetsNiva = "Brons",
                    SkapadDatum = DateTime.Now
                };

                if (_kundController.SkapaKund(nyKund))
                {
                    StatusMessage = $"Kund '{nyKund.Namn}' skapad framgångsrikt";
                    NyKundNamn = NyKundTelefon = NyKundEmail = string.Empty;
                    ValdRegion = null;
                    ValdRestaurang = null;

                    if (!string.IsNullOrWhiteSpace(SokTelefon) ||
                        !string.IsNullOrWhiteSpace(SokNamn) ||
                        !string.IsNullOrWhiteSpace(SokEmail))
                        SokKund();
                }
                else StatusMessage = "Kunde inte skapa kunden. Kontrollera telefonnummer.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid skapande av kund: {ex.Message}";
            }
        }

        [RelayCommand]
        private void RedigeraKund()
        {
            if (ValdKund == null)
            {
                StatusMessage = "Ingen kund vald för redigering";
                return;
            }

            try
            {
                ValdKund.RegionID = ValdKund.Region?.RegionID;
                ValdKund.HemmarestaurangID = ValdKund.Hemmarestaurang?.RestaurangID;

                if (_kundController.UppdateraKund(ValdKund))
                {
                    StatusMessage = $"Kund '{ValdKund.Namn}' uppdaterad";
                    SokKund();
                }
                else StatusMessage = "Kunde inte uppdatera kunden.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid uppdatering av kund: {ex.Message}";
            }
        }

        [RelayCommand]
        private void TaBortKund()
        {
            if (ValdKund == null)
            {
                StatusMessage = "Ingen kund vald för borttagning";
                return;
            }

            var result = MessageBox.Show($"Vill du verkligen ta bort kund '{ValdKund.Namn}'?",
                                         "Bekräfta borttagning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (_kundController.TaBortKund(ValdKund.KundID))
                    {
                        StatusMessage = $"Kund '{ValdKund.Namn}' borttagen";
                        HittadeKunder.Remove(ValdKund);
                        ValdKund = null;
                        AntalHittadeKunder = HittadeKunder.Count;
                    }
                    else StatusMessage = "Kunde inte ta bort kunden.";
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

        public void Dispose()
        {
            _extraController.Dispose();
        }
    }
}
