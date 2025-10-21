using AffärsLager.Controllers;
using AffärsLager.Services;
using AffärsLager.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace PresentationsLager.ViewModels.VD
{
    public partial class VDWindowViewModel : ObservableObject
    {
        private readonly StatistikController _statistikController;
        private readonly LoggController _loggController;
        private readonly PdfController _pdfController;
        private readonly BokforingService _bokforingService;
        private readonly RestaurangController _restaurangController;

        [ObservableProperty]
        private Anvandare? inloggadAnvandare;

        // Restaurangval
        [ObservableProperty]
        private ObservableCollection<RestaurangValDto> tillgangligaRestauranger = new();

        [ObservableProperty]
        private int valdRestaurangId = 0;

        // Period
        [ObservableProperty]
        private DateTime franDatum = DateTime.Today.AddDays(-7);

        [ObservableProperty]
        private DateTime tillDatum = DateTime.Today;

        [ObservableProperty]
        private string valdPeriod = "Vecka";

        [ObservableProperty]
        private ObservableCollection<string> tillgangligaPerioder = new() { "Idag", "Vecka", "Månad", "Anpassad" };

        // Koncernöversikt
        [ObservableProperty]
        private decimal totalForsaljningKoncern = 0;

        [ObservableProperty]
        private decimal matSummaKoncern = 0;

        [ObservableProperty]
        private decimal alkoholSummaKoncern = 0;

        [ObservableProperty]
        private int antalTransaktionerKoncern = 0;

        // Procent och bredder
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
        private ObservableCollection<RegionStatistikDto> regionStatistik = new();

        [ObservableProperty]
        private ObservableCollection<MenyStatistikDto> mestSaldaRatterKoncern = new();

        [ObservableProperty]
        private ObservableCollection<MenyStatistikDto> minstSaldaRatterKoncern = new();

        [ObservableProperty]
        private ObservableCollection<RestaurangStatistikDto> restaurangStatistik = new();

        [ObservableProperty]
        private string statusMeddelande = string.Empty;

        // ===== NYA DIAGRAM-PROPERTIES =====

        // Stapeldiagram - Top 10 mest sålda rätter
        [ObservableProperty]
        private SeriesCollection topRatterChart = new();

        [ObservableProperty]
        private string[] topRatterLabels = Array.Empty<string>();

        // Cirkeldiagram - Mat vs Alkohol fördelning
        [ObservableProperty]
        private SeriesCollection kategoriFordelningChart = new();

        // Linjediagram - Försäljning över tid (daglig trend)
        [ObservableProperty]
        private SeriesCollection forsaljningsTrendChart = new();

        [ObservableProperty]
        private string[] trendLabels = Array.Empty<string>();

        // Stapeldiagram - Regionjämförelse
        [ObservableProperty]
        private SeriesCollection regionJamforelseChart = new();

        [ObservableProperty]
        private string[] regionLabels = Array.Empty<string>();

        public Action? CloseAction { get; set; }

        public VDWindowViewModel()
        {
            _statistikController = new StatistikController();
            _loggController = new LoggController();
            _pdfController = new PdfController();
            _bokforingService = new AffärsLager.Services.BokforingService();
            _restaurangController = new RestaurangController();
        }

        public void Initialize(Anvandare anvandare)
        {
            InloggadAnvandare = anvandare;
            LaddaRestauranger();
            ValdPeriod = "Vecka";
            UppdateraStatistik();

            _loggController.LoggaHandelse(
                anvandare.AnvandarID,
                "VD",
                "Öppnade VD-vy",
                "Koncernöversikt"
            );
        }

        private void LaddaRestauranger()
        {
            try
            {
                TillgangligaRestauranger.Add(new RestaurangValDto
                {
                    RestaurangID = 0,
                    Restaurangnamn = "🏢 Hela Koncernen"
                });

                var restauranger = _restaurangController.HamtaAllaRestauranger();
                foreach (var rest in restauranger.OrderBy(r => r.Restaurangnamn))
                {
                    TillgangligaRestauranger.Add(new RestaurangValDto
                    {
                        RestaurangID = rest.RestaurangID,
                        Restaurangnamn = rest.Restaurangnamn
                    });
                }

                ValdRestaurangId = 0;
            }
            catch (Exception ex)
            {
                StatusMeddelande = $"Fel vid laddning av restauranger: {ex.Message}";
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

        partial void OnValdRestaurangIdChanged(int value)
        {
            UppdateraStatistik();
        }

        [RelayCommand]
        private void UppdateraStatistik()
        {
            try
            {
                if (InloggadAnvandare == null) return;

                StatusMeddelande = "Laddar statistik...";

                if (ValdRestaurangId == 0)
                {
                    VisaKoncernStatistik();
                }
                else
                {
                    VisaRestaurangStatistik(ValdRestaurangId);
                }

                // Uppdatera diagram
                UppdateraDiagram();

                StatusMeddelande = $"Statistik uppdaterad {FranDatum:yyyy-MM-dd} - {TillDatum:yyyy-MM-dd}";
            }
            catch (Exception ex)
            {
                StatusMeddelande = $"Fel: {ex.Message}";
                MessageBox.Show($"Fel vid statistik: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VisaKoncernStatistik()
        {
            var koncernSummary = _statistikController.HamtaKoncernStatistik(
                FranDatum,
                TillDatum,
                InloggadAnvandare!.AnvandarID
            );

            TotalForsaljningKoncern = koncernSummary.TotalForsaljning;
            MatSummaKoncern = koncernSummary.MatForsaljning;
            AlkoholSummaKoncern = koncernSummary.DryckForsaljning;
            AntalTransaktionerKoncern = koncernSummary.TotaltAntalBestallningar;

            if (TotalForsaljningKoncern > 0)
            {
                MatProcent = (double)(MatSummaKoncern / TotalForsaljningKoncern * 100);
                AlkoholProcent = (double)(AlkoholSummaKoncern / TotalForsaljningKoncern * 100);
                MatBredd = MatProcent * 7.5;
                AlkoholBredd = AlkoholProcent * 7.5;
            }

            MestSaldaRatterKoncern.Clear();
            foreach (var meny in koncernSummary.MestSaldaRatter.Take(10))
            {
                MestSaldaRatterKoncern.Add(meny);
            }

            MinstSaldaRatterKoncern.Clear();
            foreach (var meny in koncernSummary.MinstSaldaRatter.Take(10))
            {
                MinstSaldaRatterKoncern.Add(meny);
            }

            ByggRegionStatistik();
            ByggRestaurangStatistik();
        }

        private void VisaRestaurangStatistik(int restaurangId)
        {
            var summary = _statistikController.HamtaForsaljningsSummary(
                restaurangId,
                FranDatum,
                TillDatum,
                InloggadAnvandare!.AnvandarID
            );

            TotalForsaljningKoncern = summary.TotalForsaljning;
            MatSummaKoncern = summary.MatForsaljning;
            AlkoholSummaKoncern = summary.DryckForsaljning;
            AntalTransaktionerKoncern = summary.TotaltAntalBestallningar;

            if (TotalForsaljningKoncern > 0)
            {
                MatProcent = (double)(MatSummaKoncern / TotalForsaljningKoncern * 100);
                AlkoholProcent = (double)(AlkoholSummaKoncern / TotalForsaljningKoncern * 100);
                MatBredd = MatProcent * 7.5;
                AlkoholBredd = AlkoholProcent * 7.5;
            }

            MestSaldaRatterKoncern.Clear();
            foreach (var meny in summary.MestSaldaRatter.Take(10))
            {
                MestSaldaRatterKoncern.Add(meny);
            }

            MinstSaldaRatterKoncern.Clear();
            foreach (var meny in summary.MinstSaldaRatter.Take(10))
            {
                MinstSaldaRatterKoncern.Add(meny);
            }

            RegionStatistik.Clear();
            RestaurangStatistik.Clear();
        }

        private void ByggRegionStatistik()
        {
            RegionStatistik.Clear();

            var regioner = new[] {
                new { RegionID = 1, Namn = "Norr", RestIds = new[] { 1, 2 } },
                new { RegionID = 2, Namn = "Öst", RestIds = new[] { 3, 4, 5, 6, 7, 8, 9 } },
                new { RegionID = 3, Namn = "Väst", RestIds = new[] { 10, 11, 12, 13, 14 } },
                new { RegionID = 4, Namn = "Syd", RestIds = new[] { 15, 16, 17, 18 } }
            };

            foreach (var region in regioner)
            {
                decimal totalForsaljning = 0;
                decimal matSumma = 0;
                decimal alkoholSumma = 0;
                int antalTrans = 0;

                foreach (var restId in region.RestIds)
                {
                    try
                    {
                        var summary = _statistikController.HamtaForsaljningsSummary(
                            restId, FranDatum, TillDatum, InloggadAnvandare!.AnvandarID
                        );

                        totalForsaljning += summary.TotalForsaljning;
                        matSumma += summary.MatForsaljning;
                        alkoholSumma += summary.DryckForsaljning;
                        antalTrans += summary.TotaltAntalBestallningar;
                    }
                    catch { }
                }

                RegionStatistik.Add(new RegionStatistikDto
                {
                    RegionNamn = region.Namn,
                    TotalForsaljning = totalForsaljning,
                    MatSumma = matSumma,
                    AlkoholSumma = alkoholSumma,
                    AntalTransaktioner = antalTrans
                });
            }
        }

        private void ByggRestaurangStatistik()
        {
            RestaurangStatistik.Clear();

            for (int restId = 1; restId <= 18; restId++)
            {
                try
                {
                    var summary = _statistikController.HamtaForsaljningsSummary(
                        restId, FranDatum, TillDatum, InloggadAnvandare!.AnvandarID
                    );

                    RestaurangStatistik.Add(new RestaurangStatistikDto
                    {
                        RestaurangNamn = summary.RestaurangNamn,
                        TotalForsaljning = summary.TotalForsaljning,
                        MatSumma = summary.MatForsaljning,
                        AlkoholSumma = summary.DryckForsaljning,
                        AntalTransaktioner = summary.TotaltAntalBestallningar
                    });
                }
                catch { }
            }
        }

        // ===== NYA DIAGRAM-METODER =====

        private void UppdateraDiagram()
        {
            ByggTopRatterChart();
            ByggKategoriFordelningChart();
            ByggForsaljningsTrendChart();
            ByggRegionJamforelseChart();
        }

        private void ByggTopRatterChart()
        {
            TopRatterChart = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Antal sålda",
                    Values = new ChartValues<int>(MestSaldaRatterKoncern.Take(10).Select(m => m.AntalSalda)),
                    Fill = new SolidColorBrush(Color.FromRgb(16, 44, 72)), // #102c48
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:N0}"
                }
            };

            TopRatterLabels = MestSaldaRatterKoncern.Take(10).Select(m => m.Rattnamn).ToArray();
        }

        private void ByggKategoriFordelningChart()
        {
            KategoriFordelningChart = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Mat",
                    Values = new ChartValues<decimal> { MatSummaKoncern },
                    Fill = new SolidColorBrush(Color.FromRgb(16, 44, 72)), // #102c48
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:N0} kr ({point.Participation:P0})"
                },
                new PieSeries
                {
                    Title = "Alkohol",
                    Values = new ChartValues<decimal> { AlkoholSummaKoncern },
                    Fill = new SolidColorBrush(Color.FromRgb(88, 129, 87)), // #588157
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:N0} kr ({point.Participation:P0})"
                }
            };
        }

        private void ByggForsaljningsTrendChart()
        {
            // Beräkna daglig försäljning för perioden
            var dagar = new List<DateTime>();
            var forsaljningPerDag = new List<decimal>();

            for (var dag = FranDatum; dag <= TillDatum; dag = dag.AddDays(1))
            {
                dagar.Add(dag);

                decimal dagsForsaljning = 0;

                if (ValdRestaurangId == 0)
                {
                    // Koncern - summera alla restauranger
                    for (int restId = 1; restId <= 18; restId++)
                    {
                        try
                        {
                            var dagsSummary = _statistikController.HamtaForsaljningsSummary(
                                restId, dag, dag, InloggadAnvandare!.AnvandarID
                            );
                            dagsForsaljning += dagsSummary.TotalForsaljning;
                        }
                        catch { }
                    }
                }
                else
                {
                    // Specifik restaurang
                    try
                    {
                        var dagsSummary = _statistikController.HamtaForsaljningsSummary(
                            ValdRestaurangId, dag, dag, InloggadAnvandare!.AnvandarID
                        );
                        dagsForsaljning = dagsSummary.TotalForsaljning;
                    }
                    catch { }
                }

                forsaljningPerDag.Add(dagsForsaljning);
            }

            ForsaljningsTrendChart = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Försäljning",
                    Values = new ChartValues<decimal>(forsaljningPerDag),
                    Stroke = new SolidColorBrush(Color.FromRgb(244, 185, 66)), // #F4B942
                    Fill = new SolidColorBrush(Color.FromArgb(50, 244, 185, 66)),
                    StrokeThickness = 3,
                    PointGeometrySize = 8,
                    DataLabels = false
                }
            };

            TrendLabels = dagar.Select(d => d.ToString("MM-dd")).ToArray();
        }

        private void ByggRegionJamforelseChart()
        {
            RegionJamforelseChart = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total Försäljning",
                    Values = new ChartValues<decimal>(RegionStatistik.Select(r => r.TotalForsaljning)),
                    Fill = new SolidColorBrush(Color.FromRgb(163, 177, 138)), // #A3B18A
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:N0} kr"
                }
            };

            RegionLabels = RegionStatistik.Select(r => r.RegionNamn).ToArray();
        }

        [RelayCommand]
        private void GeneraPDFRapport()
        {
            try
            {
                StatusMeddelande = "Genererar PDF...";

                string pdfFil;

                if (ValdRestaurangId == 0)
                {
                    pdfFil = _pdfController.GeneraKoncernRapport(
                        FranDatum, TillDatum, InloggadAnvandare!.AnvandarID
                    );
                }
                else
                {
                    pdfFil = _pdfController.GeneraStatistikRapport(
                        ValdRestaurangId, FranDatum, TillDatum, InloggadAnvandare!.AnvandarID
                    );
                }

                MessageBox.Show(
                    $"PDF-rapport sparad!\n\n{pdfFil}",
                    "PDF genererad",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                StatusMeddelande = "PDF-rapport genererad";
            }
            catch (Exception ex)
            {
                StatusMeddelande = $"Fel: {ex.Message}";
                MessageBox.Show($"Fel: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async void SkickaPDFPerMail()
        {
            try
            {
                StatusMeddelande = "Skickar PDF via email...";

                bool skickad;

                if (ValdRestaurangId == 0)
                {
                    var pdfFil = _pdfController.GeneraKoncernRapport(
                        FranDatum, TillDatum, InloggadAnvandare!.AnvandarID
                    );

                    skickad = await new AffärsLager.Services.EmailService().SkickaEmail(
                        "vd@restonation.se",
                        "Koncernrapport RestoNation",
                        $"<h2>Koncernrapport</h2><p>Period: {FranDatum:yyyy-MM-dd} - {TillDatum:yyyy-MM-dd}</p>",
                        new System.Collections.Generic.List<string> { pdfFil }
                    );
                }
                else
                {
                    skickad = await _pdfController.GeneraOchSkickaPdfRapport(
                        ValdRestaurangId, FranDatum, TillDatum,
                        "vd@restonation.se", InloggadAnvandare!.AnvandarID
                    );
                }

                if (skickad)
                {
                    MessageBox.Show("PDF skickad till vd@restonation.se", "Email skickat",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMeddelande = "PDF skickad via email";
                }
                else
                {
                    MessageBox.Show("Email-fel. Kontrollera SMTP-inställningar.", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                StatusMeddelande = $"Fel: {ex.Message}";
                MessageBox.Show($"Fel: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
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

    public class RestaurangValDto
    {
        public int RestaurangID { get; set; }
        public string Restaurangnamn { get; set; } = string.Empty;
    }

    public class RegionStatistikDto
    {
        public string RegionNamn { get; set; } = string.Empty;
        public decimal TotalForsaljning { get; set; }
        public decimal MatSumma { get; set; }
        public decimal AlkoholSumma { get; set; }
        public int AntalTransaktioner { get; set; }
    }

    public class RestaurangStatistikDto
    {
        public string RestaurangNamn { get; set; } = string.Empty;
        public decimal TotalForsaljning { get; set; }
        public decimal MatSumma { get; set; }
        public decimal AlkoholSumma { get; set; }
        public int AntalTransaktioner { get; set; }
    }
}