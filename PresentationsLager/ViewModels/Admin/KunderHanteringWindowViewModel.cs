using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using PresentationsLager.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace PresentationsLager.ViewModels.Admin
{
    public partial class KunderHanteringWindowViewModel : ObservableObject
    {
        private readonly KundController _kundController;
        private readonly RegionController _regionController;
        private readonly RestaurangController _restaurangController;
        private readonly ExtraController _extraController;

        [ObservableProperty] private string sokTelefon = string.Empty;
        [ObservableProperty] private string sokNamn = string.Empty;
        [ObservableProperty] private string sokEmail = string.Empty;

        [ObservableProperty] private string nyKundNamn = string.Empty;
        [ObservableProperty] private string nyKundTelefon = string.Empty;
        [ObservableProperty] private string nyKundEmail = string.Empty;

        [ObservableProperty] private RegionModel? valdRegion;
        [ObservableProperty] private RestaurangModel? valdRestaurang;
        [ObservableProperty] private KundModel? valdKund;
        [ObservableProperty] private string statusMessage = string.Empty;
        [ObservableProperty] private int antalHittadeKunder = 0;

        [ObservableProperty] private RegionModel? redigeradRegion;
        [ObservableProperty] private RestaurangModel? redigeradRestaurang;
        [ObservableProperty] private ObservableCollection<RestaurangModel> redigeringsRestauranger = new();
        [ObservableProperty] private ObservableCollection<string> lojalitetsNivaer = new() { "Brons", "Silver", "Guld" };
        [ObservableProperty] private string nyStatusMessage = string.Empty;

        [ObservableProperty] private ObservableCollection<KundModel> hittadeKunder = new();
        [ObservableProperty] private ObservableCollection<RegionModel> regioner = new();
        [ObservableProperty] private ObservableCollection<RestaurangModel> restauranger = new();

        private bool _isLoadingKund = false;

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
                    Regioner.Add(RegionModel.FromEntity(region));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av regioner: {ex.Message}";
            }
        }

        partial void OnValdRegionChanged(RegionModel? value)
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
                    Restauranger.Add(RestaurangModel.FromEntity(r));
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
                    HittadeKunder.Add(KundModel.FromEntity(kund));

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
                NyKundTelefon = NyKundTelefon.Trim();
                NyKundTelefon = NyKundTelefon.Replace(" ", "");
                StatusMessage = string.Empty;
                string errorMessages = "";

                if (string.IsNullOrWhiteSpace(NyKundNamn) || string.IsNullOrWhiteSpace(NyKundEmail))
                {
                    errorMessages = "Fyll i de obligatoriska fälten";
                }

                if (NyKundTelefon.Length < 7 || NyKundTelefon.Length > 15 || !NyKundTelefon.All(char.IsDigit))
                {
                    errorMessages += "\n\nOgiltigt telefonnummer! Skriv bara siffror (7–15 tecken).";
                }

                if (!Regex.IsMatch(NyKundEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    errorMessages += "\n\nOgiltig e-postadress! Ange en giltig adress, t.ex. namn@domän.se.";
                }

                if (!string.IsNullOrEmpty(errorMessages))
                {
                    MessageBox.Show(errorMessages, "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    NyKundNamn = string.Empty;
                    NyKundTelefon = string.Empty;
                    NyKundEmail = string.Empty;
                    ValdRegion = null;
                    ValdRestaurang = null;

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
                NyStatusMessage = $"Fel vid skapande av kund: {ex.Message}";
            }
        }

        partial void OnValdKundChanged(KundModel? kund)
        {
            if (kund == null) return;

            _isLoadingKund = true;

            kund.PropertyChanged += ValdKund_PropertyChanged;

            if (kund.RegionID.HasValue)
            {
                RedigeradRegion = Regioner.FirstOrDefault(r => r.RegionID == kund.RegionID.Value);
                if (RedigeradRegion != null)
                    LoadRedigeringsRestauranger(RedigeradRegion.RegionID);
            }

            if (kund.HemmarestaurangID.HasValue)
                RedigeradRestaurang = RedigeringsRestauranger.FirstOrDefault(r => r.RestaurangID == kund.HemmarestaurangID.Value);

            _isLoadingKund = false;
        }

        private void ValdKund_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_isLoadingKund || ValdKund == null) return;

            try
            {
                if (e.PropertyName == nameof(ValdKund.Namn) ||
                    e.PropertyName == nameof(ValdKund.Telefon) ||
                    e.PropertyName == nameof(ValdKund.Email) ||
                    e.PropertyName == nameof(ValdKund.LojalitetsPoang) ||
                    e.PropertyName == nameof(ValdKund.LojalitetsNiva))
                {
                    // Om poängen ändras, uppdatera automatiskt nivån
                    if (e.PropertyName == nameof(ValdKund.LojalitetsPoang))
                    {
                        var nyNiva = AffärsLager.Services.LojalitetsService.BeraknaLojalitetsNiva(ValdKund.LojalitetsPoang);
                        if (ValdKund.LojalitetsNiva != nyNiva)
                        {
                            ValdKund.LojalitetsNiva = nyNiva;
                        }
                    }

                    _kundController.UppdateraKund(ValdKund.ToEntity());
                    StatusMessage = "Ändringar sparade.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid sparande: {ex.Message}";
            }
        }



        partial void OnRedigeradRegionChanged(RegionModel? value)
        {
            if (_isLoadingKund || ValdKund == null) return;

            try
            {
                if (value != null)
                {
                    LoadRedigeringsRestauranger(value.RegionID);
                    ValdKund.RegionID = value.RegionID;
                    _kundController.UppdateraKund(ValdKund.ToEntity());
                    StatusMessage = $"Region uppdaterad för '{ValdKund.Namn}'.";
                }
                else
                {
                    RedigeringsRestauranger.Clear();
                    ValdKund.RegionID = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid byte av region: {ex.Message}";
            }
        }

        partial void OnRedigeradRestaurangChanged(RestaurangModel? value)
        {
            if (_isLoadingKund || ValdKund == null) return;

            try
            {
                ValdKund.HemmarestaurangID = value?.RestaurangID;
                _kundController.UppdateraKund(ValdKund.ToEntity());
                StatusMessage = $"Hemmarestaurang uppdaterad för '{ValdKund.Namn}'.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid byte av restaurang: {ex.Message}";
            }
        }

        private void LoadRedigeringsRestauranger(int regionId)
        {
            try
            {
                RedigeringsRestauranger.Clear();
                foreach (var r in _restaurangController.HamtaRestaurangerForRegion(regionId))
                    RedigeringsRestauranger.Add(RestaurangModel.FromEntity(r));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av restauranger: {ex.Message}";
            }
        }

        [RelayCommand]
        private void TaBortKund()
        {
            if (ValdKund == null)
            {
                StatusMessage = "Ingen kundvald för borttagning";
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
