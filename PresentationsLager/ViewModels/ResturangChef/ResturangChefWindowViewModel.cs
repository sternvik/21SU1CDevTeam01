using AffärsLager.Controllers;
using AffärsLager.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PresentationsLager.ViewModels.ResturangChef
{
    public partial class ResturangChefWindowViewModel : ObservableObject
    {
        private readonly StatistikController _statistikController;
        private readonly LoggController _loggController;
        private readonly PdfController _pdfController;
        private readonly AffärsLager.Services.BokforingService _bokforingService;

        [ObservableProperty]
        private Anvandare? inloggadAnvandare;

        [ObservableProperty]
        private string restaurangNamn = string.Empty;

        [ObservableProperty]
        private DateTime franDatum = DateTime.Today.AddDays(-7);

        [ObservableProperty]
        private DateTime tillDatum = DateTime.Today;

        [ObservableProperty]
        private string valdPeriod = "Vecka";

        [ObservableProperty]
        private ObservableCollection<string> tillgangligaPerioder = new() { "Idag", "Vecka", "Månad", "Anpassad" };

        // Sammanfattning
        [ObservableProperty]
        private decimal totalForsaljning = 0;

        [ObservableProperty]
        private decimal matSumma = 0;

        [ObservableProperty]
        private decimal alkoholSumma = 0;

        [ObservableProperty]
        private int antalTransaktioner = 0;

        [ObservableProperty]
        private int antalBokningar = 0;

        [ObservableProperty]
        private int antalGaster = 0;

        [ObservableProperty]
        private int antalUnikalaBord = 0;

        // Procent och bredder för visualisering (progress bars)
        [ObservableProperty]
        private double matProcent = 0;

        [ObservableProperty]
        private double alkoholProcent = 0;

        [ObservableProperty]
        private double matBredd = 0;

        [ObservableProperty]
        private double alkoholBredd = 0;

        // Statistik samlingar
        [ObservableProperty]
        private ObservableCollection<MenyStatistikDto> mestSaldaRatter = new();

        [ObservableProperty]
        private ObservableCollection<MenyStatistikDto> minstSaldaRatter = new();

        [ObservableProperty]
        private ObservableCollection<ServitorDto> servitorStatistik = new();

        [ObservableProperty]
        private string statusMeddelande = string.Empty;

        public Action? CloseAction { get; set; }

        public ResturangChefWindowViewModel()
        {
            _statistikController = new StatistikController();
            _loggController = new LoggController();
            _pdfController = new PdfController();
            _bokforingService = new AffärsLager.Services.BokforingService();
        }

        public void Initialize(Anvandare anvandare)
        {
            InloggadAnvandare = anvandare;

            if (anvandare.HemmarestaurangID.HasValue)
            {
                var restController = new AffärsLager.Controllers.RestaurangController();
                var restaurang = restController.HamtaRestaurangMedId(anvandare.HemmarestaurangID.Value);
                RestaurangNamn = restaurang?.Restaurangnamn ?? "Okänd restaurang";

                ValdPeriod = "Vecka";
                UppdateraStatistik();

                _loggController.LoggaHandelse(
                    anvandare.AnvandarID,
                    "RestaurangChef",
                    "Öppnade statistikvy",
                    $"Restaurang: {RestaurangNamn}"
                );
            }
            else
            {
                StatusMeddelande = "Ingen hemmarestaurang angiven för användaren";
            }
        }

        partial void OnValdPeriodChanged(string value)
        {
            switch (value)
            {
                case "Idag":
                    FranDatum = DateTime.Today;
                    TillDatum = DateTime.Today;
                    break;
                case "Vecka":
                    FranDatum = DateTime.Today.AddDays(-7);
                    TillDatum = DateTime.Today;
                    break;
                case "Månad":
                    FranDatum = DateTime.Today.AddMonths(-1);
                    TillDatum = DateTime.Today;
                    break;
                case "Anpassad":
                    return;
            }

            UppdateraStatistik();
        }

        [RelayCommand]
        private void UppdateraStatistik()
        {
            try
            {
                if (InloggadAnvandare?.HemmarestaurangID == null)
                {
                    StatusMeddelande = "Ingen restaurang vald";
                    return;
                }

                StatusMeddelande = "Laddar statistik...";

                var summary = _statistikController.HamtaForsaljningsSummary(
                    InloggadAnvandare.HemmarestaurangID.Value,
                    FranDatum,
                    TillDatum,
                    InloggadAnvandare.AnvandarID
                );

                // Uppdatera sammanfattning
                TotalForsaljning = summary.TotalForsaljning;
                MatSumma = summary.MatForsaljning;
                AlkoholSumma = summary.DryckForsaljning;
                AntalBokningar = summary.TotaltAntalBokningar;
                AntalGaster = summary.TotaltAntalGaster;
                AntalTransaktioner = summary.TotaltAntalBestallningar;

                // Beräkna procent och bredder för progress bars
                if (TotalForsaljning > 0)
                {
                    MatProcent = (double)(MatSumma / TotalForsaljning * 100);
                    AlkoholProcent = (double)(AlkoholSumma / TotalForsaljning * 100);
                    MatBredd = MatProcent * 7.5; // Max 750px
                    AlkoholBredd = AlkoholProcent * 7.5;
                }

                // Uppdatera rättstatistik
                MestSaldaRatter.Clear();
                foreach (var meny in summary.MestSaldaRatter.Take(10))
                {
                    MestSaldaRatter.Add(meny);
                }

                MinstSaldaRatter.Clear();
                foreach (var meny in summary.MinstSaldaRatter.Take(10))
                {
                    MinstSaldaRatter.Add(meny);
                }

                // Uppdatera servitörstatistik
                ServitorStatistik.Clear();
                foreach (var personal in summary.PersonalStatistik)
                {
                    ServitorStatistik.Add(new ServitorDto
                    {
                        Namn = personal.PersonalNamn,
                        AntalTransaktioner = personal.AntalBokningar, // Använd bokningar som proxy för transaktioner
                        TotalForsaljning = personal.TotalForsaljning
                    });
                }

                // Beräkna unika bord
                AntalUnikalaBord = summary.PersonalStatistik.Sum(p => p.AntalBordHanterade);

                StatusMeddelande = $"Statistik uppdaterad för {FranDatum:yyyy-MM-dd} - {TillDatum:yyyy-MM-dd}";
            }
            catch (Exception ex)
            {
                StatusMeddelande = $"Fel: {ex.Message}";
                MessageBox.Show($"Fel vid hämtning av statistik: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void GeneraPDFRapport()
        {
            try
            {
                if (InloggadAnvandare?.HemmarestaurangID == null)
                {
                    MessageBox.Show("Ingen restaurang vald", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                StatusMeddelande = "Genererar PDF...";

                var pdfFil = _pdfController.GeneraStatistikRapport(
                    InloggadAnvandare.HemmarestaurangID.Value,
                    FranDatum,
                    TillDatum,
                    InloggadAnvandare.AnvandarID
                );

                MessageBox.Show(
                    $"PDF-rapport sparad!\n\nPlats: {pdfFil}",
                    "PDF genererad",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                StatusMeddelande = "PDF-rapport genererad";
            }
            catch (Exception ex)
            {
                StatusMeddelande = $"Fel: {ex.Message}";
                MessageBox.Show($"Fel vid PDF-generering: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async void SkickaPDFPerMail()
        {
            try
            {
                if (InloggadAnvandare?.HemmarestaurangID == null)
                {
                    MessageBox.Show("Ingen restaurang vald", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                StatusMeddelande = "Skickar PDF via email...";

                var skickad = await _pdfController.GeneraOchSkickaPdfRapport(
                    InloggadAnvandare.HemmarestaurangID.Value,
                    FranDatum,
                    TillDatum,
                    "restaurangchef@restonation.se",
                    InloggadAnvandare.AnvandarID
                );

                if (skickad)
                {
                    MessageBox.Show(
                        "PDF-rapport skickad till restaurangchef@restonation.se",
                        "Email skickat",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    StatusMeddelande = "PDF skickad via email";
                }
                else
                {
                    MessageBox.Show(
                        "Kunde inte skicka email. Kontrollera email-inställningar.",
                        "Email-fel",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    StatusMeddelande = "Fel vid email-sändning";
                }
            }
            catch (Exception ex)
            {
                StatusMeddelande = $"Fel: {ex.Message}";
                MessageBox.Show($"Fel vid email-sändning: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void LoggaUt()
        {
            var result = MessageBox.Show(
                $"Vill du logga ut {InloggadAnvandare?.Namn}?",
                "Logga ut",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                _loggController.LoggaUtloggning(
                    InloggadAnvandare?.AnvandarID ?? 0,
                    InloggadAnvandare?.Anvandarnamn ?? ""
                );

                var loginWindow = new Views.LoginWindow();
                loginWindow.Show();
                CloseAction?.Invoke();
            }
        }
    }

    // Helper DTO för servitörstatistik i XAML
    public class ServitorDto
    {
        public string Namn { get; set; } = string.Empty;
        public int AntalTransaktioner { get; set; }
        public decimal TotalForsaljning { get; set; }
    }
}