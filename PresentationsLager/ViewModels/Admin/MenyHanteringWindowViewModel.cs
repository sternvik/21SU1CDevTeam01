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
        [ObservableProperty] private string? nyKategori = "À la carte";

        // Lista med tillgängliga kategorier
        public List<string> Kategorier { get; } = new List<string>
        {
            "À la carte",
            "Dagens lunch",
            "Dryck"
        };

        [ObservableProperty] private string valdMenyPrisText = string.Empty;
        [ObservableProperty] private string? nyStatusMessage;
        [ObservableProperty] private string? statusMessage;

        public Action? CloseAction { get; set; }

        public MenyHanteringWindowViewModel()
        {
            LoadRegioner();
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
                var list = _restaurangMenyController.HamtaMenyForRestaurang(ValdRestaurang.RestaurangID)
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
            if (ValdRestaurang == null)
            {
                StatusMessage = "Välj restaurang först.";
                return;
            }

            try
            {
                // Hämta BARA menyer för den valda restaurangen
                var menyerForRestaurang = _restaurangMenyController.HamtaMenyForRestaurang(ValdRestaurang.RestaurangID);

                // Filtrera på sökkriterier
                var filtered = menyerForRestaurang.AsQueryable();

                if (!string.IsNullOrWhiteSpace(SokRattnamn))
                    filtered = filtered.Where(m => m.Rattnamn.ToLower().Contains(SokRattnamn.Trim().ToLower()));

                if (!string.IsNullOrWhiteSpace(SokKategori))
                    filtered = filtered.Where(m => m.Kategori.ToLower().Contains(SokKategori.Trim().ToLower()));

                var list = filtered.Select(MenyModel.FromEntity).ToList();
                Menyvaror = new ObservableCollection<MenyModel>(list);
                StatusMessage = $"Hittade {list.Count} rätt(er) för {ValdRestaurang.Restaurangnamn}.";
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
                // Använd ny controller för att undvika EF caching
                var freshController = new RestaurangMenyController();
                var list = freshController
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

        private void UppdateraMenyLista()
        {
            if (ValdRestaurang != null)
            {
                OnValdRestaurangChanged(ValdRestaurang);
            }
        }

        partial void OnValdMenyChanged(MenyModel? value)
        {
            if (value != null)
            {
                ValdMenyPrisText = value.Pris.ToString(System.Globalization.CultureInfo.InvariantCulture);
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

                if (!decimal.TryParse(NyPrisText.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var pris))
                {
                    NyStatusMessage = "Pris måste vara ett giltigt tal.";
                    return;
                }

                // Skapa ny rätt - ALLTID restaurangspecifik och aktiv
                var meny = new Meny
                {
                    Rattnamn = NyRattnamn!.Trim(),
                    Beskrivning = NyBeskrivning?.Trim() ?? string.Empty,
                    Pris = pris,
                    Kategori = NyKategori!.Trim(),
                    ArGrundmeny = false,
                    Aktiv = true
                };

                _menyController.SkapaMeny(meny);

                // MenyID är nu satt efter Save() i SkapaMeny
                _restaurangMenyController.KopplaMenyTillRestaurang(ValdRestaurang.RestaurangID, meny.MenyID);

                NyStatusMessage = $"Rätten '{meny.Rattnamn}' lades till för {ValdRestaurang.Restaurangnamn}.";

                // Rensa formuläret
                NyRattnamn = string.Empty;
                NyBeskrivning = string.Empty;
                NyPrisText = string.Empty;
                NyKategori = "À la carte";

                // Uppdatera listan med färska data
                UppdateraMenyLista();
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
                if (!decimal.TryParse(ValdMenyPrisText.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var pris))
                {
                    StatusMessage = "Pris måste vara ett giltigt tal.";
                    return;
                }

                // Kolla om rätten används av flera restauranger
                int antalRestauranger = _restaurangMenyController.RaknaAntalRestaurangerSomAnvanderMeny(ValdMeny.MenyID);

                if (antalRestauranger > 1)
                {
                    // Skapa en KOPIA av rätten för denna restaurang (påverkar inte andra restauranger)
                    var nyMeny = new Meny
                    {
                        Rattnamn = ValdMeny.Rattnamn,
                        Beskrivning = ValdMeny.Beskrivning ?? string.Empty,
                        Pris = pris,
                        Kategori = ValdMeny.Kategori,
                        ArGrundmeny = false,
                        Aktiv = true
                    };

                    _menyController.SkapaMeny(nyMeny);

                    // Ta bort gamla kopplingen och skapa ny till kopian
                    _restaurangMenyController.TaBortMenyFranRestaurang(ValdRestaurang.RestaurangID, ValdMeny.MenyID);
                    _restaurangMenyController.KopplaMenyTillRestaurang(ValdRestaurang.RestaurangID, nyMeny.MenyID);

                    StatusMessage = $"Rätten '{nyMeny.Rattnamn}' uppdaterades för {ValdRestaurang.Restaurangnamn}.";
                }
                else
                {
                    // Uppdatera direkt (bara denna restaurang använder rätten)
                    ValdMeny.Pris = pris;
                    _menyController.UppdateraMeny(ValdMeny.ToEntity());

                    StatusMessage = $"Rätten '{ValdMeny.Rattnamn}' uppdaterades.";
                }

                // Uppdatera listan med färska data
                UppdateraMenyLista();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid uppdatering: {ex.Message}";
            }
        }

        [RelayCommand]
        private void TaBortMeny()
        {
            if (ValdMeny == null || ValdRestaurang == null)
            {
                StatusMessage = "Välj restaurang och rätt först.";
                return;
            }

            try
            {
                var rattnamn = ValdMeny.Rattnamn;
                var menyId = ValdMeny.MenyID;

                // Ta bort RestaurangMeny-kopplingen för denna restaurang
                _restaurangMenyController.TaBortMenyFranRestaurang(ValdRestaurang.RestaurangID, menyId);

                // Kolla om rätten används av andra restauranger
                int antalRestauranger = _restaurangMenyController.RaknaAntalRestaurangerSomAnvanderMeny(menyId);

                if (antalRestauranger == 0)
                {
                    // Ingen restaurang använder rätten längre - ta bort från databasen helt
                    _menyController.TaBortMeny(menyId);
                    StatusMessage = $"Rätten '{rattnamn}' togs bort från {ValdRestaurang.Restaurangnamn} och databasen.";
                }
                else
                {
                    StatusMessage = $"Rätten '{rattnamn}' togs bort från {ValdRestaurang.Restaurangnamn}.";
                }

                ValdMeny = null;

                // Uppdatera listan med färska data
                UppdateraMenyLista();
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
