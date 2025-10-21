using AffärsLager.Controllers;
using AffärsLager.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PresentationsLager.ViewModels.VD
{
    public partial class VDWindowViewModel : ObservableObject
    {
        private readonly StatistikController _statistikController;
        private readonly LoggController _loggController;
        private readonly PdfController _pdfController;
        private readonly AffärsLager.Services.BokforingService _bokforingService;
        private readonly RestaurangController _restaurangController;

        [ObservableProperty]
        private Anvandare? inloggadAnvandare;

        // Restaurangval
        [ObservableProperty]
        private ObservableCollection<RestaurangValDto> tillgangligaRestauranger = new();

        [ObservableProperty]
        private int valdRestaurangId = 0; // 0 = Hela koncernen

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
                // Lägg till "Hela Koncernen"
                TillgangligaRestauranger.Add(new RestaurangValDto
                {
                    RestaurangID = 0,
                    Restaurangnamn = "🏢 Hela Koncernen"
                });

                // Hämta alla restauranger
                var restauranger = _restaurangController.HamtaAllaRestauranger();
                foreach (var rest in restauranger.OrderBy(r => r.Restaurangnamn))
                {
                    TillgangligaRestauranger.Add(new RestaurangValDto
                    {
                        RestaurangID = rest.RestaurangID,
                        Restaurangnamn = rest.Restaurangnamn
                    });
                }

                ValdRestaurangId = 0; // Standard: Hela koncernen
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
                    // HELA KONCERNEN
                    VisaKoncernStatistik();
                }
                else
                {
                    // SPECIFIK RESTAURANG
                    VisaRestaurangStatistik(ValdRestaurangId);
                }

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

            // Uppdatera sammanfattning
            TotalForsaljningKoncern = koncernSummary.TotalForsaljning;
            MatSummaKoncern = koncernSummary.MatForsaljning;
            AlkoholSummaKoncern = koncernSummary.DryckForsaljning;
            AntalTransaktionerKoncern = koncernSummary.TotaltAntalBestallningar;

            // Beräkna procent
            if (TotalForsaljningKoncern > 0)
            {
                MatProcent = (double)(MatSummaKoncern / TotalForsaljningKoncern * 100);
                AlkoholProcent = (double)(AlkoholSummaKoncern / TotalForsaljningKoncern * 100);
                MatBredd = MatProcent * 7.5;
                AlkoholBredd = AlkoholProcent * 7.5;
            }

            // Uppdatera rättstatistik
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

            // Bygg regionstatistik
            ByggRegionStatistik();

            // Bygg restaurangstatistik
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

    // Helper DTOs för XAML-bindningar
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