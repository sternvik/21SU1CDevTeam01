using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataLager;
using EntitetsLager;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class KundSearchWindowViewModel : ObservableObject, IDisposable
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

        public KundSearchWindowViewModel()
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
                var regionList = _regionController.HamtaAllaRegioner();
                Regioner.Clear();
                foreach (var region in regionList)
                {
                    Regioner.Add(region);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av regioner: {ex.Message}";
            }
        }

        partial void OnValdRegionChanged(Region? value)
        {
            if (value != null)
            {
                LoadRestaurangerForRegion(value.RegionID);
            }
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
                var restaurangList = _restaurangController.HamtaRestaurangerForRegion(regionId);
                Restauranger.Clear();
                foreach (var restaurang in restaurangList)
                {
                    Restauranger.Add(restaurang);
                }
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

                var resultat = _kundController.SokKunder(SokTelefon, SokNamn, SokEmail);

                HittadeKunder.Clear();
                foreach (var kund in resultat)
                {
                    HittadeKunder.Add(kund);
                }

                AntalHittadeKunder = HittadeKunder.Count;

                if (AntalHittadeKunder == 0)
                {
                    StatusMessage = "Inga kunder hittades med angivna sökkriterier";
                }
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

                bool skapad = _kundController.SkapaKund(nyKund);

                if (skapad)
                {
                    StatusMessage = $"Kund '{nyKund.Namn}' har skapats framgångsrikt";

                    // Rensa formuläret
                    NyKundNamn = string.Empty;
                    NyKundTelefon = string.Empty;
                    NyKundEmail = string.Empty;
                    ValdRegion = null;
                    ValdRestaurang = null;

                    // Uppdatera sökningen om det finns sökkriterier
                    if (!string.IsNullOrWhiteSpace(SokTelefon) ||
                        !string.IsNullOrWhiteSpace(SokNamn) ||
                        !string.IsNullOrWhiteSpace(SokEmail))
                    {
                        SokKund();
                    }
                }
                else
                {
                    StatusMessage = "Kunde inte skapa kunden. Kontrollera att telefonnumret inte redan finns.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid skapande av kund: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ValjKund()
        {
            if (ValdKund != null)
            {
                var result = MessageBox.Show(
                    $"Vill du välja kunden:\n\n{ValdKund.Namn}\nTelefon: {ValdKund.Telefon}\nLojalitetspoäng: {ValdKund.LojalitetsPoang}",
                    "Välj kund",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Här skulle vi kunna skicka tillbaka vald kund till huvudfönstret
                    // För nu visar vi bara en bekräftelse
                    MessageBox.Show($"Kund '{ValdKund.Namn}' har valts för aktuell beställning.",
                        "Kund vald", MessageBoxButton.OK, MessageBoxImage.Information);

                    CloseAction?.Invoke();
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
            // Se till att DbContext disposas korrekt för att frigöra databas-resurser
            _extraController.Dispose();
        }
    }
}