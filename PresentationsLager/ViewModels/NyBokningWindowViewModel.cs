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
        private ObservableCollection<BordViewModel> ledigaBord = new();

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

        private void UpdateLedigaBord()
        {
            try
            {
                LedigaBord.Clear();
                AntalLedigaBord = 0;

                if (!ValtDatum.HasValue || ValdTid == null || _inloggadAnvandare?.HemmarestaurangID == null)
                    return;

                var ledigaBordLista = _bokningsController.HamtaLedigaBord(
                    _inloggadAnvandare.HemmarestaurangID.Value,
                    ValtDatum.Value,
                    ValdTid.Tid,
                    ValtAntalGaster);

                foreach (var bord in ledigaBordLista)
                {
                    var isLampligt = bord.AntalPlatser >= ValtAntalGaster;
                    LedigaBord.Add(new BordViewModel
                    {
                        BordID = bord.BordID,
                        Bordkod = bord.Bordkod,
                        AntalPlatser = bord.AntalPlatser,
                        StatusText = isLampligt ? "Lämpligt" : "För litet",
                        StatusColor = isLampligt ? "#A3B18A" : "#E74C3C"
                    });
                }

                AntalLedigaBord = LedigaBord.Count;
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
                // Avmarkera alla andra bord
                foreach (var bord in LedigaBord)
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
                    var result = MessageBox.Show(
                        $"Bokning skapad framgångsrikt!\n\n" +
                        $"Kund: {ValdKund.Namn}\n" +
                        $"Datum: {ValtDatum:yyyy-MM-dd}\n" +
                        $"Tid: {ValdTid.DisplayText}\n" +
                        $"Bord: {ValtBord.Bordkod} ({ValtBord.AntalPlatser} platser)\n" +
                        $"Antal gäster: {ValtAntalGaster}\n\n" +
                        "Vill du skapa en ny bokning?",
                        "Bokning skapad",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Rensa formuläret för ny bokning
                        RensaFormular();
                    }
                    else
                    {
                        CloseAction?.Invoke();
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
        public string StatusColor { get; set; } = "#A3B18A"; // Green for available

        [ObservableProperty]
        private bool isSelected = false;
    }
}