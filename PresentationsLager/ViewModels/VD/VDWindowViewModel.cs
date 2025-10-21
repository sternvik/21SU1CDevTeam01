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
    public partial class VDWindowViewModel : ObservableObject
    {
        private readonly StatistikController _statistikController;
        private readonly AnvandareController _anvandareController;
        private readonly PDFService _pdfService;
        private readonly MailService _mailService;

        [ObservableProperty]
        private Anvandare? inloggadAnvandare;

        [ObservableProperty]
        private string valdPeriod = "Vecka";

        [ObservableProperty]
        private int? valdRestaurangId;

        public ObservableCollection<RestaurangValjare> TillgangligaRestauranger { get; } = new();

        [ObservableProperty]
        private DateTime startDatum;

        [ObservableProperty]
        private DateTime slutDatum;

        // Koncernöversikt
        [ObservableProperty]
        private decimal totalForsaljningKoncern;

        [ObservableProperty]
        private decimal matSummaKoncern;

        [ObservableProperty]
        private decimal alkoholSummaKoncern;

        [ObservableProperty]
        private int antalTransaktionerKoncern;

        // Regionstatistik
        [ObservableProperty]
        private ObservableCollection<RegionStatistikViewModel> regionStatistik = new();

        // Rättstatistik koncern
        [ObservableProperty]
        private ObservableCollection<RattStatistikViewModel> mestSaldaRatterKoncern = new();

        [ObservableProperty]
        private ObservableCollection<RattStatistikViewModel> minstSaldaRatterKoncern = new();

        // Restaurangstatistik
        [ObservableProperty]
        private ObservableCollection<RestaurangStatistikViewModel> restaurangStatistik = new();

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

        public VDWindowViewModel()
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

            // Ladda alla restauranger för väljare
            var restaurangController = new RestaurangController();
            var restauranger = restaurangController.HamtaAllaRestauranger();

            TillgangligaRestauranger.Clear();
            TillgangligaRestauranger.Add(new RestaurangValjare { RestaurangID = null, Restaurangnamn = "Hela Koncernen" });
            foreach (var rest in restauranger)
            {
                TillgangligaRestauranger.Add(new RestaurangValjare
                {
                    RestaurangID = rest.RestaurangID,
                    Restaurangnamn = rest.Restaurangnamn
                });
            }

            // Sätt default till Hela Koncernen
            ValdRestaurangId = null;

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

        partial void OnValdRestaurangIdChanged(int? value)
        {
            LaddaStatistik();
        }

        [RelayCommand]
        private void LaddaStatistik()
        {
            try
            {
                if (ValdRestaurangId == null)
                {
                    // KONCERNÖVERSIKT
                    var forsaljningKoncern = _statistikController.GetForsaljningKoncern(StartDatum, SlutDatum);
                    TotalForsaljningKoncern = forsaljningKoncern.TotalForsaljning;
                    MatSummaKoncern = forsaljningKoncern.MatSumma;
                    AlkoholSummaKoncern = forsaljningKoncern.AlkoholSumma;
                    AntalTransaktionerKoncern = forsaljningKoncern.AntalTransaktioner;

                    // Hämta regionstatistik
                    var regioner = _statistikController.GetForsaljningPerRegion(StartDatum, SlutDatum);
                    RegionStatistik.Clear();
                    foreach (var region in regioner)
                    {
                        RegionStatistik.Add(new RegionStatistikViewModel
                        {
                            RegionNamn = region.RegionNamn,
                            TotalForsaljning = region.TotalForsaljning,
                            MatSumma = region.MatSumma,
                            AlkoholSumma = region.AlkoholSumma,
                            AntalTransaktioner = region.AntalTransaktioner
                        });
                    }

                    // Hämta mest sålda rätter koncern
                    var mestSalda = _statistikController.GetMestSaldaRatterKoncern(StartDatum, SlutDatum, 10);
                    MestSaldaRatterKoncern.Clear();
                    foreach (var ratt in mestSalda)
                    {
                        MestSaldaRatterKoncern.Add(new RattStatistikViewModel
                        {
                            Rattnamn = ratt.Rattnamn,
                            Kategori = ratt.Kategori,
                            AntalSalda = ratt.AntalSalda,
                            TotalForsaljning = ratt.TotalForsaljning
                        });
                    }

                    // Hämta minst sålda rätter koncern
                    var minstSalda = _statistikController.GetMinstSaldaRatterKoncern(StartDatum, SlutDatum, 10);
                    MinstSaldaRatterKoncern.Clear();
                    foreach (var ratt in minstSalda)
                    {
                        MinstSaldaRatterKoncern.Add(new RattStatistikViewModel
                        {
                            Rattnamn = ratt.Rattnamn,
                            Kategori = ratt.Kategori,
                            AntalSalda = ratt.AntalSalda,
                            TotalForsaljning = ratt.TotalForsaljning
                        });
                    }

                    // Hämta restaurangstatistik
                    var restauranger = _statistikController.GetForsaljningPerRestaurang(StartDatum, SlutDatum);
                    RestaurangStatistik.Clear();
                    foreach (var restaurang in restauranger)
                    {
                        RestaurangStatistik.Add(new RestaurangStatistikViewModel
                        {
                            RestaurangNamn = restaurang.RestaurangNamn,
                            TotalForsaljning = restaurang.TotalForsaljning,
                            MatSumma = restaurang.MatSumma,
                            AlkoholSumma = restaurang.AlkoholSumma,
                            AntalTransaktioner = restaurang.AntalTransaktioner
                        });
                    }

                    // Beräkna procentsatser för visualisering
                    if (TotalForsaljningKoncern > 0)
                    {
                        MatProcent = (double)(MatSummaKoncern / TotalForsaljningKoncern) * 100;
                        AlkoholProcent = (double)(AlkoholSummaKoncern / TotalForsaljningKoncern) * 100;
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
                else
                {
                    // ENSKILD RESTAURANG
                    int restaurangId = ValdRestaurangId.Value;

                    var forsaljning = _statistikController.GetForsaljningRestaurang(restaurangId, StartDatum, SlutDatum);
                    TotalForsaljningKoncern = forsaljning.TotalForsaljning;
                    MatSummaKoncern = forsaljning.MatSumma;
                    AlkoholSummaKoncern = forsaljning.AlkoholSumma;
                    AntalTransaktionerKoncern = forsaljning.AntalTransaktioner;

                    // Töm regionstatistik (inte relevant för enskild restaurang)
                    RegionStatistik.Clear();

                    // Hämta mest sålda rätter för restaurangen
                    var mestSalda = _statistikController.GetMestSaldaRatter(restaurangId, StartDatum, SlutDatum, 10);
                    MestSaldaRatterKoncern.Clear();
                    foreach (var ratt in mestSalda)
                    {
                        MestSaldaRatterKoncern.Add(new RattStatistikViewModel
                        {
                            Rattnamn = ratt.Rattnamn,
                            Kategori = ratt.Kategori,
                            AntalSalda = ratt.AntalSalda,
                            TotalForsaljning = ratt.TotalForsaljning
                        });
                    }

                    // Hämta minst sålda rätter för restaurangen
                    var minstSalda = _statistikController.GetMinstSaldaRatter(restaurangId, StartDatum, SlutDatum, 10);
                    MinstSaldaRatterKoncern.Clear();
                    foreach (var ratt in minstSalda)
                    {
                        MinstSaldaRatterKoncern.Add(new RattStatistikViewModel
                        {
                            Rattnamn = ratt.Rattnamn,
                            Kategori = ratt.Kategori,
                            AntalSalda = ratt.AntalSalda,
                            TotalForsaljning = ratt.TotalForsaljning
                        });
                    }

                    // Töm restaurangstatistik (inte relevant för enskild restaurang)
                    RestaurangStatistik.Clear();

                    // Beräkna procentsatser för visualisering
                    if (TotalForsaljningKoncern > 0)
                    {
                        MatProcent = (double)(MatSummaKoncern / TotalForsaljningKoncern) * 100;
                        AlkoholProcent = (double)(AlkoholSummaKoncern / TotalForsaljningKoncern) * 100;
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
                // Hämta all statistik
                var koncernForsaljning = _statistikController.GetForsaljningKoncern(StartDatum, SlutDatum);
                var regioner = _statistikController.GetForsaljningPerRegion(StartDatum, SlutDatum);
                var mestSaldaKoncern = _statistikController.GetMestSaldaRatterKoncern(StartDatum, SlutDatum, 10);
                var restauranger = _statistikController.GetForsaljningPerRestaurang(StartDatum, SlutDatum);

                // Generera PDF
                string pdfPath = _pdfService.GenerateVDRapport(
                    StartDatum,
                    SlutDatum,
                    koncernForsaljning,
                    regioner,
                    mestSaldaKoncern,
                    restauranger);

                MessageBox.Show($"VD-rapport genererad!\n\nFilen sparad: {pdfPath}", "Framgång",
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
                // Hämta all statistik
                var koncernForsaljning = _statistikController.GetForsaljningKoncern(StartDatum, SlutDatum);
                var regioner = _statistikController.GetForsaljningPerRegion(StartDatum, SlutDatum);
                var mestSaldaKoncern = _statistikController.GetMestSaldaRatterKoncern(StartDatum, SlutDatum, 10);
                var restauranger = _statistikController.GetForsaljningPerRestaurang(StartDatum, SlutDatum);

                // Generera PDF
                string pdfPath = _pdfService.GenerateVDRapport(
                    StartDatum,
                    SlutDatum,
                    koncernForsaljning,
                    regioner,
                    mestSaldaKoncern,
                    restauranger);

                // Skicka mail
                string period = $"{StartDatum:yyyy-MM-dd} till {SlutDatum:yyyy-MM-dd}";
                bool success = await _mailService.SendStatistikRapportAsync(
                    "leosternvik@gmail.com",
                    "RestoNation Koncernen",
                    period,
                    pdfPath);

                if (success)
                {
                    MessageBox.Show($"VD-rapport skickad till leosternvik@gmail.com!", "Framgång",
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

    // ViewModel-klasser för VD-statistik
    public class RegionStatistikViewModel
    {
        public string RegionNamn { get; set; } = string.Empty;
        public decimal TotalForsaljning { get; set; }
        public decimal MatSumma { get; set; }
        public decimal AlkoholSumma { get; set; }
        public int AntalTransaktioner { get; set; }
    }

    public class RestaurangStatistikViewModel
    {
        public string RestaurangNamn { get; set; } = string.Empty;
        public decimal TotalForsaljning { get; set; }
        public decimal MatSumma { get; set; }
        public decimal AlkoholSumma { get; set; }
        public int AntalTransaktioner { get; set; }
    }

    public class RestaurangValjare
    {
        public int? RestaurangID { get; set; }
        public string Restaurangnamn { get; set; } = string.Empty;
    }
}