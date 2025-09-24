using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataLager;
using EntitetsLager;
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
        private readonly UnitOfWork _unitOfWork;
        private Anvandare? _inloggadAnvandare;

        [ObservableProperty]
        private DateTime? valtDatum = DateTime.Today;

        [ObservableProperty]
        private TidOption? valdTid;

        [ObservableProperty]
        private int valtAntalGaster = 2;

        [ObservableProperty]
        private Kund? valdKund;

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
            _unitOfWork = new UnitOfWork();
            _bokningsController = new BokningsController(_unitOfWork);
            _bordController = new BordController(_unitOfWork);

            InitializeData();
        }

        public void Initialize(Anvandare anvandare, Kund? forvaldKund = null)
        {
            _inloggadAnvandare = anvandare;

            // Sätt förvald kund om en angavs
            if (forvaldKund != null)
            {
                ValdKund = forvaldKund;
                ValdKundText = $"{forvaldKund.Namn} - {forvaldKund.Telefon}";
            }

            UpdateLedigaBord();
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

                if (!ValtDatum.HasValue || ValdTid == null || _inloggadAnvandare?.HemmarestaurangID == null)
                    return;

                IEnumerable<BordMedStatus> bordMedStatus;

                // Tvinga uppdatering av databas-cache endast när det behövs
                if (refreshFromDatabase)
                {
                    // Skapa en helt ny UnitOfWork för att säkerställa färska data från databasen
                    using (var freshUnitOfWork = new UnitOfWork())
                    {
                        var freshBokningsController = new BokningsController(freshUnitOfWork);
                        bordMedStatus = freshBokningsController.HamtaAllaBordMedStatus(
                            _inloggadAnvandare.HemmarestaurangID.Value,
                            ValtDatum.Value,
                            ValdTid.Tid,
                            ValtAntalGaster);
                    }
                }
                else
                {
                    bordMedStatus = _bokningsController.HamtaAllaBordMedStatus(
                        _inloggadAnvandare.HemmarestaurangID.Value,
                        ValtDatum.Value,
                        ValdTid.Tid,
                        ValtAntalGaster);
                }

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
                            statusText = "Bokat";
                            bokningsTidText = $"{startTid:hh\\:mm}-{slutTid:hh\\:mm}";
                        }
                        else
                        {
                            statusText = "Bokat";
                        }
                        statusColor = "#E74C3C"; // Röd för bokade bord
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
                        BokningsTidText = bokningsTidText
                    });
                }

                AntalLedigaBord = AllaBord.Count(b => b.ArLedigt);
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
                // Kontrollera att bordet är ledigt
                if (!bordViewModel.ArLedigt)
                {
                    StatusMessage = "Detta bord är redan bokat för den valda tiden";
                    return;
                }

                // Kontrollera att bordet är tillräckligt stort
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
                if (!ValtDatum.HasValue)
                {
                    StatusMessage = "Datum måste anges";
                    return;
                }

                if (ValdTid == null)
                {
                    StatusMessage = "Tid måste anges";
                    return;
                }

                if (ValdKund == null)
                {
                    StatusMessage = "Kund måste väljas";
                    return;
                }

                if (ValtBord == null)
                {
                    StatusMessage = "Bord måste väljas";
                    return;
                }

                if (_inloggadAnvandare?.HemmarestaurangID == null)
                {
                    StatusMessage = "Ingen restaurang angiven för användaren";
                    return;
                }

                // Skapa ny bokning
                var bokning = new Bokning
                {
                    KundID = ValdKund.KundID,
                    BordID = ValtBord.BordID,
                    RestaurangID = _inloggadAnvandare.HemmarestaurangID.Value,
                    AnvandarID = _inloggadAnvandare.AnvandarID,
                    Datum = ValtDatum.Value,
                    Tid = ValdTid.Tid,
                    AntalGaster = ValtAntalGaster,
                    Specialinformation = string.IsNullOrWhiteSpace(Specialinformation) ? null : Specialinformation.Trim(),
                    BokningsTyp = "På plats"
                };

                bool skapad = _bokningsController.SkapaBokning(bokning);

                if (skapad)
                {
                    // Uppdatera bordvyn direkt för att visa den nya bokningen
                    UpdateLedigaBord(refreshFromDatabase: true);

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

        [RelayCommand]
        private void Tillbaka()
        {
            CloseAction?.Invoke();
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
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

        [ObservableProperty]
        private bool isSelected = false;
    }
}