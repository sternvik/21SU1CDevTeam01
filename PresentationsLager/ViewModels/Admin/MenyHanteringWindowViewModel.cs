using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AffärsLager.Controllers;
using EntitetsLager;
using PresentationsLager.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PresentationsLager.ViewModels.Admin
{
    public partial class MenyHanteringWindowViewModel : ObservableObject
    {
        private readonly MenyController _menyController = new MenyController();
        private readonly RegionController _regionController = new RegionController();
        private readonly RestaurangController _restaurangController = new RestaurangController();
        private readonly RestaurangMenyController _restaurangMenyController = new RestaurangMenyController();

        [ObservableProperty] private ObservableCollection<Region> regioner = new();
        [ObservableProperty] private ObservableCollection<Restaurang> restauranger = new();
        [ObservableProperty] private ObservableCollection<MenyModel> menyvaror = new();

        [ObservableProperty] private Region? valdRegion;
        [ObservableProperty] private Restaurang? valdRestaurang;
        [ObservableProperty] private MenyModel? valdMeny;

        [ObservableProperty] private string? sokRattnamn;
        [ObservableProperty] private string? sokKategori;

        [ObservableProperty] private string? nyRattnamn;
        [ObservableProperty] private string? nyBeskrivning;
        [ObservableProperty] private string nyPrisText = string.Empty;
        [ObservableProperty] private string? nyKategori;
        [ObservableProperty] private bool nyArGrundmeny = true;
        [ObservableProperty] private bool nyAktiv = true;

        [ObservableProperty] private string valdMenyPrisText = string.Empty;
        [ObservableProperty] private string? nyStatusMessage;
        [ObservableProperty] private string? statusMessage;

        public Action? CloseAction { get; set; }

        public MenyHanteringWindowViewModel()
        {
            LoadRegioner();
            LaddaAllaMenyer();
        }

        private void LoadRegioner()
        {
            try
            {
                Regioner.Clear();
                foreach (var r in _regionController.HamtaAllaRegioner())
                    Regioner.Add(r);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av regioner: {ex.Message}";
            }
        }

        partial void OnValdRegionChanged(Region? value)
        {
            try
            {
                Restauranger.Clear();
                ValdRestaurang = null;
                if (value != null)
                {
                    foreach (var r in _restaurangController.HamtaRestaurangerForRegion(value.RegionID))
                        Restauranger.Add(r);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av restauranger: {ex.Message}";
            }
        }

        private void LaddaAllaMenyer()
        {
            try
            {
                var list = _menyController.HamtaAllaMenyvaror()
                    .Select(MenyModel.FromEntity)
                    .ToList();
                Menyvaror = new ObservableCollection<MenyModel>(list);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av menyer: {ex.Message}";
            }
        }

        [RelayCommand]
        private void LaddaMeny()
        {
            if (ValdRestaurang == null)
            {
                StatusMessage = "Välj restaurang först.";
                return;
            }

            try
            {
                var list = _menyController.HamtaMenyvarorForRestaurang(ValdRestaurang.RestaurangID)
                    .Select(MenyModel.FromEntity)
                    .ToList();
                Menyvaror = new ObservableCollection<MenyModel>(list);
                StatusMessage = $"Meny laddad för {ValdRestaurang.Restaurangnamn}.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av meny: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SokMeny()
        {
            try
            {
                var list = _menyController.SokMeny(SokRattnamn, SokKategori)
                    .Select(MenyModel.FromEntity)
                    .ToList();
                Menyvaror = new ObservableCollection<MenyModel>(list);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid sökning: {ex.Message}";
            }
        }

        partial void OnValdRestaurangChanged(Restaurang? value)
        {
            if (value == null)
            {
                Menyvaror.Clear();
                return;
            }

            try
            {
                var list = _restaurangMenyController
                    .HamtaMenyForRestaurang(value.RestaurangID)
                    .Select(MenyModel.FromEntity)
                    .ToList();

                Menyvaror = new ObservableCollection<MenyModel>(list);
                StatusMessage = $"Meny laddad för {value.Restaurangnamn}.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid laddning av meny: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SkapaMeny()
        {
            try
            {
                if (ValdRestaurang == null)
                {
                    NyStatusMessage = "Välj restaurang först.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(NyRattnamn) ||
                    string.IsNullOrWhiteSpace(NyKategori) ||
                    string.IsNullOrWhiteSpace(NyPrisText))
                {
                    NyStatusMessage = "Fyll i alla obligatoriska fält.";
                    return;
                }

                if (!decimal.TryParse(NyPrisText.Replace(',', '.'), out var pris))
                {
                    NyStatusMessage = "Pris måste vara ett giltigt tal.";
                    return;
                }

                var meny = new Meny
                {
                    Rattnamn = NyRattnamn!.Trim(),
                    Beskrivning = NyBeskrivning,
                    Pris = pris,
                    Kategori = NyKategori!.Trim(),
                    ArGrundmeny = NyArGrundmeny,
                    Aktiv = NyAktiv
                };

                _menyController.SkapaMeny(meny);
                _restaurangMenyController.KopplaMenyTillRestaurang(ValdRestaurang.RestaurangID, meny.MenyID);

                NyStatusMessage = $"Rätten '{meny.Rattnamn}' skapades för {ValdRestaurang.Restaurangnamn}.";
                OnValdRestaurangChanged(ValdRestaurang);
            }
            catch (Exception ex)
            {
                NyStatusMessage = $"Fel vid skapande: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SparaMeny()
        {
            if (ValdMeny == null || ValdRestaurang == null)
            {
                StatusMessage = "Välj restaurang och rätt först.";
                return;
            }

            try
            {
                if (!decimal.TryParse(ValdMenyPrisText.Replace(',', '.'), out var pris))
                {
                    StatusMessage = "Pris måste vara ett giltigt tal.";
                    return;
                }

                ValdMeny.Pris = pris;
                _menyController.UppdateraMeny(ValdMeny.ToEntity());
                StatusMessage = $"Rätten '{ValdMeny.Rattnamn}' uppdaterades.";
                OnValdRestaurangChanged(ValdRestaurang);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid uppdatering: {ex.Message}";
            }
        }

        [RelayCommand]
        private void TaBortMeny()
        {
            if (ValdMeny == null)
            {
                StatusMessage = "Ingen rätt vald för borttagning.";
                return;
            }

            try
            {
                _menyController.TaBortMeny(ValdMeny.MenyID);

                if (ValdRestaurang != null)
                    LaddaMeny();
                else
                    LaddaAllaMenyer();

                StatusMessage = $"Rätten '{ValdMeny.Rattnamn}' togs bort.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid borttagning: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Tillbaka()
        {
            CloseAction?.Invoke();
        }
    }
}
