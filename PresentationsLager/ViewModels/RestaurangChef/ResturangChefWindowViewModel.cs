using AffärsLager.Controllers;
using AffärsLager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using PresentationsLager.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class RestaurangChefWindowViewModel : ObservableObject
    {
        private readonly StatistikController _statistikController;
        private readonly AnvandareController _anvandareController;
        private readonly PDFService _pdfService;
        private readonly MailService _mailService;

        [ObservableProperty]
        private Anvandare? inloggadAnvandare;

        [ObservableProperty]
        private string restaurangNamn = "";

        [ObservableProperty]
        private string valdPeriod = "Vecka";

        [ObservableProperty]
        private DateTime startDatum;

        [ObservableProperty]
        private DateTime slutDatum;

        // Försäljningsstatistik
        [ObservableProperty]
        private decimal totalForsaljning;

        [ObservableProperty]
        private decimal totalDricks;

        [ObservableProperty]
        private decimal matSumma;

        [ObservableProperty]
        private decimal alkoholSumma;

        [ObservableProperty]
        private int antalTransaktioner;

        // Rättstatistik
        [ObservableProperty]
        private ObservableCollection<RattStatistikViewModel> mestSaldaRatter = new();

        [ObservableProperty]
        private ObservableCollection<RattStatistikViewModel> minstSaldaRatter = new();

        // Servitörstatistik
        [ObservableProperty]
        private ObservableCollection<ServitorStatistikViewModel> servitorStatistik = new();

        // Bokningsstatistik
        [ObservableProperty]
        private int antalBokningar;

        [ObservableProperty]
        private int antalGaster;

        [ObservableProperty]
        private int antalUnikalaBord;

        // Percentages for visualization
        [ObservableProperty]
        private double matProcent;

        [ObservableProperty]
        private double alkoholProcent;

        [ObservableProperty]
        private double matBredd; // 0-800 pixels

        [ObservableProperty]
        private double alkoholBredd; // 0-800 pixels

        // Perioder
        public ObservableCollection<string> TillgangligaPerioder { get; } = new()
        {
            "Dag",
            "Vecka",
            "Månad"
        };

        public Action? CloseAction { get; set; }

        public RestaurangChefWindowViewModel()
        {
            _statistikController = new StatistikController();
            _anvandareController = new AnvandareController();
            _pdfService = new PDFService();
            _mailService = new MailService();

            // Konfigurera SMTP för Gmail
            _mailService.ConfigureSMTP(
                host: "smtp.gmail.com",
                port: 587,
                username: "victorberg66@gmail.com",
                password: "rtci btcl twqc nzbx",
                fromEmail: "victorberg66@gmail.com",
                fromName: "RestoNation System"
            );

            // Sätt default period till innevarande vecka
            SattPeriodVecka();
        }

        public void Initialize(Anvandare anvandare)
        {
            InloggadAnvandare = anvandare;

            // Hämta restaurangnamn
            if (InloggadAnvandare != null && InloggadAnvandare.HemmarestaurangID.HasValue)
            {
                var restaurangController = new RestaurangController();
                var restaurang = restaurangController.HamtaRestaurangMedId(InloggadAnvandare.HemmarestaurangID.Value);
                RestaurangNamn = restaurang?.Restaurangnamn ?? "Okänd restaurang";
            }

            LaddaStatistik();
        }

        partial void OnValdPeriodChanged(string value)
        {
            switch (value)
            {
                case "Dag":
                    SattPeriodDag();
                    break;
                case "Vecka":
                    SattPeriodVecka();
                    break;
                case "Månad":
                    SattPeriodManad();
                    break;
            }
            LaddaStatistik();
        }

        [RelayCommand]
        private void LaddaStatistik()
        {
            try
            {
                if (InloggadAnvandare == null || !InloggadAnvandare.HemmarestaurangID.HasValue)
                {
                    MessageBox.Show("Ingen hemmarestaurang kopplad till användaren", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int restaurangId = InloggadAnvandare.HemmarestaurangID.Value;

                // Hämta försäljningsstatistik
                var forsaljning = _statistikController.GetForsaljningRestaurang(restaurangId, StartDatum, SlutDatum);
                TotalForsaljning = forsaljning.TotalForsaljning;
                TotalDricks = forsaljning.TotalDricks;
                MatSumma = forsaljning.MatSumma;
                AlkoholSumma = forsaljning.AlkoholSumma;
                AntalTransaktioner = forsaljning.AntalTransaktioner;

                // Hämta mest sålda rätter
                var mestSalda = _statistikController.GetMestSaldaRatter(restaurangId, StartDatum, SlutDatum, 10);
                MestSaldaRatter.Clear();
                foreach (var ratt in mestSalda)
                {
                    MestSaldaRatter.Add(new RattStatistikViewModel
                    {
                        Rattnamn = ratt.Rattnamn,
                        Kategori = ratt.Kategori,
                        AntalSalda = ratt.AntalSalda,
                        TotalForsaljning = ratt.TotalForsaljning
                    });
                }

                // Hämta minst sålda rätter
                var minstSalda = _statistikController.GetMinstSaldaRatter(restaurangId, StartDatum, SlutDatum, 10);
                MinstSaldaRatter.Clear();
                foreach (var ratt in minstSalda)
                {
                    MinstSaldaRatter.Add(new RattStatistikViewModel
                    {
                        Rattnamn = ratt.Rattnamn,
                        Kategori = ratt.Kategori,
                        AntalSalda = ratt.AntalSalda,
                        TotalForsaljning = ratt.TotalForsaljning
                    });
                }

                // Hämta servitörstatistik
                var servitorer = _statistikController.GetForsaljningPerServitor(restaurangId, StartDatum, SlutDatum);
                ServitorStatistik.Clear();

                // Hämta även personalstatistik från StatistikService för att få bokningsdata
                var statistikService = new StatistikService();
                var personalStatistik = statistikService.HamtaPersonalStatistik(restaurangId, StartDatum, SlutDatum);

                foreach (var servitor in servitorer)
                {
                    // Hitta motsvarande personalstatistik för att få bokningsdata
                    var personalData = personalStatistik.FirstOrDefault(p => p.AnvandarID == servitor.AnvandarID);

                    ServitorStatistik.Add(new ServitorStatistikViewModel
                    {
                        Namn = servitor.Namn,
                        AntalTransaktioner = servitor.AntalTransaktioner,
                        TotalForsaljning = servitor.TotalForsaljning,
                        TotalDricks = servitor.TotalDricks,
                        AntalBokningar = personalData?.AntalBokningar ?? 0,
                        AntalBord = personalData?.AntalBordHanterade ?? 0,
                        AntalGaster = personalData?.TotaltAntalGaster ?? 0
                    });
                }

                // Hämta bokningsstatistik
                var bokningar = _statistikController.GetBokningsStatistik(restaurangId, StartDatum, SlutDatum);
                AntalBokningar = bokningar.AntalBokningar;
                AntalGaster = bokningar.AntalGaster;
                AntalUnikalaBord = bokningar.AntalUnikalaBord;

                // Beräkna procentsatser för visualisering
                if (TotalForsaljning > 0)
                {
                    MatProcent = (double)(MatSumma / TotalForsaljning) * 100;
                    AlkoholProcent = (double)(AlkoholSumma / TotalForsaljning) * 100;
                    MatBredd = MatProcent * 8; // Max 800px
                    AlkoholBredd = AlkoholProcent * 8; // Max 800px
                }
                else
                {
                    MatProcent = 50; // Visa 50/50 när ingen data
                    AlkoholProcent = 50;
                    MatBredd = 400;
                    AlkoholBredd = 400;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid laddning av statistik: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void LoggaUt()
        {
            var result = MessageBox.Show($"Vill du logga ut {InloggadAnvandare?.Namn}?", "Logga ut",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (InloggadAnvandare != null)
                {
                    _anvandareController.LoggaUtAnvandare(InloggadAnvandare.AnvandarID);
                }

                var loginWindow = new LoginWindow();
                loginWindow.Show();
                CloseAction?.Invoke();
            }
        }

        [RelayCommand]
        private async void GeneraPDFRapport()
        {
            try
            {
                if (InloggadAnvandare == null || !InloggadAnvandare.HemmarestaurangID.HasValue)
                {
                    MessageBox.Show("Ingen hemmarestaurang kopplad till användaren", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int restaurangId = InloggadAnvandare.HemmarestaurangID.Value;

                // Hämta restaurangens namn
                var restaurangController = new RestaurangController();
                var restaurang = restaurangController.HamtaRestaurangMedId(restaurangId);
                string restaurangNamn = restaurang?.Restaurangnamn ?? "Okänd";

                // Hämta all statistik
                var forsaljning = _statistikController.GetForsaljningRestaurang(restaurangId, StartDatum, SlutDatum);
                var mestSalda = _statistikController.GetMestSaldaRatter(restaurangId, StartDatum, SlutDatum, 10);
                var minstSalda = _statistikController.GetMinstSaldaRatter(restaurangId, StartDatum, SlutDatum, 10);
                var servitorer = _statistikController.GetForsaljningPerServitor(restaurangId, StartDatum, SlutDatum);
                var bokningar = _statistikController.GetBokningsStatistik(restaurangId, StartDatum, SlutDatum);

                // Generera PDF
                string pdfPath = _pdfService.GenerateRestaurangchefRapport(
                    restaurangNamn,
                    StartDatum,
                    SlutDatum,
                    forsaljning,
                    mestSalda,
                    minstSalda,
                    servitorer,
                    bokningar);

                MessageBox.Show($"PDF-rapport genererad!\n\nFilen sparad: {pdfPath}", "Framgång",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Öppna PDF-filen
                Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid generering av PDF: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async void SkickaPDFPerMail()
        {
            try
            {
                if (InloggadAnvandare == null || !InloggadAnvandare.HemmarestaurangID.HasValue)
                {
                    MessageBox.Show("Ingen hemmarestaurang kopplad till användaren", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int restaurangId = InloggadAnvandare.HemmarestaurangID.Value;

                // Hämta restaurangens namn
                var restaurangController = new RestaurangController();
                var restaurang = restaurangController.HamtaRestaurangMedId(restaurangId);
                string restaurangNamn = restaurang?.Restaurangnamn ?? "Okänd";

                // Hämta all statistik
                var forsaljning = _statistikController.GetForsaljningRestaurang(restaurangId, StartDatum, SlutDatum);
                var mestSalda = _statistikController.GetMestSaldaRatter(restaurangId, StartDatum, SlutDatum, 10);
                var minstSalda = _statistikController.GetMinstSaldaRatter(restaurangId, StartDatum, SlutDatum, 10);
                var servitorer = _statistikController.GetForsaljningPerServitor(restaurangId, StartDatum, SlutDatum);
                var bokningar = _statistikController.GetBokningsStatistik(restaurangId, StartDatum, SlutDatum);

                // Generera PDF
                string pdfPath = _pdfService.GenerateRestaurangchefRapport(
                    restaurangNamn,
                    StartDatum,
                    SlutDatum,
                    forsaljning,
                    mestSalda,
                    minstSalda,
                    servitorer,
                    bokningar);

                // Skicka mail
                string period = $"{StartDatum:yyyy-MM-dd} till {SlutDatum:yyyy-MM-dd}";
                bool success = await _mailService.SendStatistikRapportAsync(
                    "leosternvik@gmail.com",
                    restaurangNamn,
                    period,
                    pdfPath);

                if (success)
                {
                    MessageBox.Show($"PDF-rapport skickad till leosternvik@gmail.com!", "Framgång",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Kunde inte skicka e-post. Kontrollera SMTP-inställningar.", "Varning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid skickande av mail: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SattPeriodDag()
        {
            StartDatum = DateTime.Today;
            SlutDatum = DateTime.Today.AddDays(1).AddSeconds(-1);
        }

        private void SattPeriodVecka()
        {
            var today = DateTime.Today;
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            StartDatum = today.AddDays(-daysUntilMonday);
            SlutDatum = StartDatum.AddDays(7).AddSeconds(-1);
        }

        private void SattPeriodManad()
        {
            var today = DateTime.Today;
            StartDatum = new DateTime(today.Year, today.Month, 1);
            SlutDatum = StartDatum.AddMonths(1).AddSeconds(-1);
        }
    }

    // ViewModel-klasser för statistik
    public class RattStatistikViewModel
    {
        public string Rattnamn { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public int AntalSalda { get; set; }
        public decimal TotalForsaljning { get; set; }
    }

    public class ServitorStatistikViewModel
    {
        public string Namn { get; set; } = string.Empty;
        public int AntalTransaktioner { get; set; }
        public decimal TotalForsaljning { get; set; }
        public decimal TotalDricks { get; set; }
        public int AntalBokningar { get; set; }
        public int AntalBord { get; set; }
        public int AntalGaster { get; set; }
    }
}