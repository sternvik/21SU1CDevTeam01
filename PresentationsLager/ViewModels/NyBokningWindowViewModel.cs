using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresentationsLager.Models;
using PresentationsLager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class NyBokningWindowViewModel : ObservableObject, IDisposable
    {
        private readonly BokningsController _bokningsController;
        private readonly BordController _bordController;
        private readonly RestaurangController _restaurangController;
        private readonly ExtraController _extraController;
        private AnvandareModel? _inloggadAnvandare;
        private int _valdRestaurangId;

        [ObservableProperty]
        private string restaurangNamn = string.Empty;

        [ObservableProperty]
        private DateTime? valtDatum = DateTime.Today;

        [ObservableProperty]
        private TidOption? valdTid;

        [ObservableProperty]
        private int valtAntalGaster = 2;

        [ObservableProperty]
        private KundModel? valdKund;

        [ObservableProperty]
        private string valdKundText = "Ingen kund vald";

        [ObservableProperty]
        private BordViewModel? valtBord;

        [ObservableProperty]
        private string specialinformation = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private int antalLedigaBord = 0;

        [ObservableProperty]
        private ObservableCollection<TidOption> tillgangligaTider = new();

        [ObservableProperty]
        private ObservableCollection<int> antalGasterOptions = new();

        [ObservableProperty]
        private ObservableCollection<BordViewModel> allaBord = new();

        public Action? CloseAction { get; set; }

        public NyBokningWindowViewModel()
        {
            _bokningsController = new BokningsController();
            _bordController = new BordController();
            _restaurangController = new RestaurangController();
            _extraController = new ExtraController();

            InitializeData();
        }

        public void Initialize(AnvandareModel anvandare, int restaurangId, KundModel? forvaldKund = null)
        {
            try
            {
                _inloggadAnvandare = anvandare;
                _valdRestaurangId = restaurangId;

                // DEBUG: Visa vilken restaurang som laddas
                StatusMessage = $"Laddar restaurang ID: {restaurangId}...";

                // Hämta och visa restaurangnamn
                var restaurang = _restaurangController.HamtaRestaurangMedId(restaurangId);
                RestaurangNamn = restaurang?.Restaurangnamn ?? $"Restaurang {restaurangId}";

                // Sätt förvald kund om en angavs
                if (forvaldKund != null)
                {
                    ValdKund = forvaldKund;
                    ValdKundText = $"{forvaldKund.Namn} - {forvaldKund.Telefon}";
                }

                // Säkerställ att standardvärden är satta innan vi uppdaterar borden
                if (ValtDatum == null)
                    ValtDatum = DateTime.Today;

                if (ValdTid == null && TillgangligaTider.Any())
                    ValdTid = TillgangligaTider.FirstOrDefault();

                StatusMessage = "Välj datum, tid och kund för att skapa bokningar. Klicka på bokade bord för check-in/ut.";

                UpdateLedigaBord();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid initialisering: {ex.Message}";
            }
        }

        private void InitializeData()
        {
            // Ladda tillgängliga tider (16:00-21:00)
            var tider = _bokningsController.HamtaTillgangligaTider();
            TillgangligaTider.Clear();
            foreach (var tid in tider)
            {
                var slutTid = tid.Add(TimeSpan.FromHours(2));
                TillgangligaTider.Add(new TidOption
                {
                    Tid = tid,
                    DisplayText = $"{tid:hh\\:mm} - {slutTid:hh\\:mm} (2 timmar)"
                });
            }

            // Ladda antal gäster alternativ (1-8)
            AntalGasterOptions.Clear();
            for (int i = 1; i <= 8; i++)
            {
                AntalGasterOptions.Add(i);
            }

            // Sätt standardvärden
            ValdTid = TillgangligaTider.FirstOrDefault();
        }

        partial void OnValtDatumChanged(DateTime? value)
        {
            UpdateLedigaBord();
        }

        partial void OnValdTidChanged(TidOption? value)
        {
            UpdateLedigaBord();
        }

        partial void OnValtAntalGasterChanged(int value)
        {
            UpdateLedigaBord();
        }

        private void UpdateLedigaBord(bool refreshFromDatabase = false)
        {
            try
            {
                AllaBord.Clear();
                AntalLedigaBord = 0;

                if (_valdRestaurangId <= 0)
                {
                    StatusMessage = "Ingen restaurang angiven";
                    return;
                }

                // Försök först bara hämta bord utan att filtrera på datum/tid
                if (!ValtDatum.HasValue || ValdTid == null)
                {
                    StatusMessage = "DEBUG: Visar bara bord utan filtrering";
                    var allaBordEnkelt = _bordController.HamtaBordForRestaurang(_valdRestaurangId);

                    foreach (var bord in allaBordEnkelt)
                    {
                        AllaBord.Add(new BordViewModel
                        {
                            BordID = bord.BordID,
                            Bordkod = bord.Bordkod,
                            AntalPlatser = bord.AntalPlatser,
                            StatusText = "Okänd",
                            StatusColor = "#A3B18A",
                            ArLedigt = true,
                            ArLampligt = true,
                            BokningsTidText = null,
                            BokadTid = null
                        });
                    }

                    AntalLedigaBord = AllaBord.Count;
                    StatusMessage = $"DEBUG: Visar {AllaBord.Count} bord utan filtrering";
                    return;
                }

                // Använd alltid en ny BokningsController för att undvika cache-problem
                var bordMedStatus = new BokningsController().HamtaAllaBordMedStatus(
                    _valdRestaurangId,
                    ValtDatum.Value,
                    ValdTid.Tid,
                    ValtAntalGaster);

                foreach (var bord in bordMedStatus)
                {
                    string statusText;
                    string statusColor;
                    string? bokningsTidText = null;

                    if (!bord.ArLedigt)
                    {
                        // Visa vilken tid bordet är bokat för
                        if (bord.BokadTid.HasValue)
                        {
                            var startTid = bord.BokadTid.Value;
                            var slutTid = startTid.Add(TimeSpan.FromHours(2));
                            bokningsTidText = $"{startTid:hh\\:mm}-{slutTid:hh\\:mm}";
                        }

                        // Hantera olika bokningsstatus
                        if (bord.BordStatus == "På plats")
                        {
                            statusText = "På plats";
                            statusColor = "#3498DB"; // Blå för kunder på plats
                        }
                        else
                        {
                            statusText = "Bokat";
                            statusColor = "#E74C3C"; // Röd för bokade bord
                        }
                    }
                    else if (bord.ArLampligt)
                    {
                        statusText = "Lämpligt";
                        statusColor = "#A3B18A"; // Grön för lämpliga bord
                    }
                    else
                    {
                        statusText = "För litet";
                        statusColor = "#F39C12"; // Orange för för små bord
                    }

                    AllaBord.Add(new BordViewModel
                    {
                        BordID = bord.BordID,
                        Bordkod = bord.Bordkod,
                        AntalPlatser = bord.AntalPlatser,
                        StatusText = statusText,
                        StatusColor = statusColor,
                        ArLedigt = bord.ArLedigt,
                        ArLampligt = bord.ArLampligt,
                        BokningsTidText = bokningsTidText,
                        BokadTid = bord.BokadTid
                    });
                }

                AntalLedigaBord = AllaBord.Count(b => b.ArLedigt);
                StatusMessage = "Välj datum, tid och kund för att skapa bokningar. Klicka på bokade bord för check-in/ut.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid uppdatering av bord: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ValjKund()
        {
            try
            {
                var kundSearchWindow = new KundSearchWindow();

                // Visa dialogen
                var result = kundSearchWindow.ShowDialog();

                // Hämta vald kund efter att dialogen stängts
                if (kundSearchWindow.DataContext is KundSearchWindowViewModel viewModel && viewModel.ValdKund != null)
                {
                    ValdKund = viewModel.ValdKund;
                    ValdKundText = $"{ValdKund.Namn} - {ValdKund.Telefon}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid öppning av kundsökning: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ValjBord(BordViewModel bordViewModel)
        {
            try
            {
                // Om bordet är bokat, visa bokningsdetaljer
                if (!bordViewModel.ArLedigt)
                {
                    VisaBokningsDetaljer(bordViewModel);
                    return;
                }

                // För lediga bord - kontrollera att bordet är tillräckligt stort
                if (!bordViewModel.ArLampligt)
                {
                    StatusMessage = $"Bordet har bara {bordViewModel.AntalPlatser} platser men {ValtAntalGaster} gäster ska placeras";
                    return;
                }

                // Avmarkera alla andra bord
                foreach (var bord in AllaBord)
                {
                    bord.IsSelected = false;
                }

                // Markera det valda bordet
                bordViewModel.IsSelected = true;
                ValtBord = bordViewModel;
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid val av bord: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SkapaBokning()
        {
            try
            {
                StatusMessage = string.Empty;

                // Validera input
                if (!ValtDatum.HasValue || ValdTid == null || ValdKund == null || ValtBord == null || _valdRestaurangId <= 0)
                {
                    StatusMessage = "Alla fält måste fyllas i";
                    return;
                }

                // Använd en ny BokningsController för att undvika cache-problem
                var freshBokningsController = new BokningsController();

                var bokning = new EntitetsLager.Bokning
                {
                    KundID = ValdKund.KundID,
                    BordID = ValtBord.BordID,
                    RestaurangID = _valdRestaurangId,
                    AnvandarID = _inloggadAnvandare.AnvandarID,
                    Datum = ValtDatum.Value,
                    Tid = ValdTid.Tid,
                    AntalGaster = ValtAntalGaster,
                    Specialinformation = string.IsNullOrWhiteSpace(Specialinformation) ? null : Specialinformation.Trim(),
                    BokningsTyp = "På plats"
                };

                bool skapad = freshBokningsController.SkapaBokning(bokning);

                if (skapad)
                {
                    // Uppdatera bordvyn direkt för att visa den nya bokningen
                    UpdateLedigaBord();

                    // Rensa bara bord och specialinformation, behåll kund
                    ValtBord = null;
                    Specialinformation = string.Empty;
                    StatusMessage = string.Empty;

                    // Avmarkera alla bord
                    foreach (var bord in AllaBord)
                    {
                        bord.IsSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid skapande av bokning: {ex.Message}";
            }
        }

        private void RensaFormular()
        {
            ValtDatum = DateTime.Today;
            ValdTid = TillgangligaTider.FirstOrDefault();
            ValtAntalGaster = 2;
            ValdKund = null;
            ValdKundText = "Ingen kund vald";
            ValtBord = null;
            Specialinformation = string.Empty;
            StatusMessage = string.Empty;

            // Rensa bordmarkeringar
            foreach (var bord in AllaBord)
            {
                bord.IsSelected = false;
            }
        }

        private void VisaBokningsDetaljer(BordViewModel bordViewModel)
        {
            try
            {
                if (!ValtDatum.HasValue || _valdRestaurangId <= 0)
                {
                    StatusMessage = "Kan inte visa bokningsdetaljer - saknar datum information";
                    return;
                }

                TimeSpan bokningsTid;
                if (bordViewModel.BokadTid.HasValue)
                {
                    bokningsTid = bordViewModel.BokadTid.Value;
                }
                else if (ValdTid != null)
                {
                    bokningsTid = ValdTid.Tid;
                }
                else
                {
                    StatusMessage = "Kan inte visa bokningsdetaljer - saknar tid information";
                    return;
                }

                // Använd en ny BokningsController för att undvika cache-problem
                var freshBokningsController = new BokningsController();
                var bokning = freshBokningsController.HamtaBokningForBord(
                    bordViewModel.BordID,
                    ValtDatum.Value,
                    bokningsTid);

                if (bokning == null)
                {
                    StatusMessage = "Kunde inte hitta bokning för detta bord";
                    return;
                }

                // Konvertera till BokningModel och använd nya constructorn
                var bokningModel = BokningModel.FromEntity(bokning);
                var bokningsDetaljerWindow = new BokningsDetaljerWindow(bokningModel, _inloggadAnvandare!);

                var result = bokningsDetaljerWindow.ShowDialog();

                // Uppdatera bordvyn efter att fönstret stängts
                UpdateLedigaBord();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid visning av bokningsdetaljer: {ex.Message}";
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

    public class TidOption
    {
        public TimeSpan Tid { get; set; }
        public string DisplayText { get; set; } = string.Empty;
    }

    public partial class BordViewModel : ObservableObject
    {
        public int BordID { get; set; }
        public string Bordkod { get; set; } = string.Empty;
        public int AntalPlatser { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public string StatusColor { get; set; } = "#A3B18A";
        public bool ArLedigt { get; set; } = true;
        public bool ArLampligt { get; set; } = true;
        public string? BokningsTidText { get; set; }
        public TimeSpan? BokadTid { get; set; }

        [ObservableProperty]
        private bool isSelected = false;
    }
}