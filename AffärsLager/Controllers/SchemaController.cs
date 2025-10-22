using AffärsLager.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// SchemaController hanterar schemalagda jobb för statistik och export
    ///
    /// OBS: StartaSchemalagdaJobb() är INTE aktiverad i nuläget.
    /// Avkommentera anropet i App.xaml.cs när systemet ska gå live.
    /// </summary>
    public class SchemaController : IDisposable
    {
        private readonly List<Timer> _timers = new();
        private readonly StatistikService _statistikService;
        private readonly PDFService _pdfService;
        private readonly MailService _mailService;
        private readonly BokföringsService _bokföringsService;
        private readonly KundExportService _kundExportService;
        private readonly LoggService _loggService;

        public SchemaController()
        {
            _statistikService = new StatistikService();
            _pdfService = new PDFService();
            _mailService = new MailService();
            _bokföringsService = new BokföringsService();
            _kundExportService = new KundExportService();
            _loggService = new LoggService();
        }

        /// <summary>
        /// Startar schemalagda jobb för daglig statistik (15:00, 20:00, 23:00)
        /// och veckovis kundexport (måndagar 09:00)
        ///
        /// OBS: ANROPA INTE DENNA METOD ÄN!
        /// Den kommer att skicka mail automatiskt varje dag.
        /// Aktivera endast vid live-drift.
        /// </summary>
        public void StartaSchemalagdaJobb()
        {
            // Schemalägg för 15:00, 20:00 och 23:00
            SchemaläggDagligRapport(15, 0); // 15:00
            SchemaläggDagligRapport(20, 0); // 20:00
            SchemaläggDagligRapport(23, 0); // 23:00

            // Schemalägg veckovis kundexport (måndagar kl 09:00)
            SchemaläggVeckovisKundExport(1, 9, 0); // Måndag 09:00
        }


        private void SchemaläggDagligRapport(int timme, int minut)
        {
            var timer = new Timer();
            timer.Elapsed += async (sender, e) => await GenereraDagligRapport();
            timer.Interval = BeräknaIntervall(timme, minut);
            timer.AutoReset = true;
            timer.Start();

            _timers.Add(timer);
        }

        private void SchemaläggVeckovisKundExport(int veckodag, int timme, int minut)
        {
            var timer = new Timer();
            timer.Elapsed += async (sender, e) => await GenereraVeckovisKundExport();
            timer.Interval = BeräknaVeckointervall(veckodag, timme, minut);
            timer.AutoReset = true;
            timer.Start();

            _timers.Add(timer);
        }

        private double BeräknaIntervall(int timme, int minut)
        {
            var nu = DateTime.Now;
            var målTid = new DateTime(nu.Year, nu.Month, nu.Day, timme, minut, 0);

            if (målTid < nu)
                målTid = målTid.AddDays(1);

            return (målTid - nu).TotalMilliseconds;
        }

        private double BeräknaVeckointervall(int veckodag, int timme, int minut)
        {
            var nu = DateTime.Now;
            var daysUntilTarget = ((int)veckodag - (int)nu.DayOfWeek + 7) % 7;
            var målTid = nu.Date.AddDays(daysUntilTarget).AddHours(timme).AddMinutes(minut);

            if (målTid < nu)
                målTid = målTid.AddDays(7);

            return (målTid - nu).TotalMilliseconds;
        }

        private async Task GenereraDagligRapport()
        {
            try
            {
                // Hämta alla restauranger
                var restaurangController = new RestaurangController();
                var restauranger = restaurangController.HamtaAllaRestauranger();

                foreach (var restaurang in restauranger)
                {
                    var igår = DateTime.Today.AddDays(-1);

                    // Generera statistik
                    var statistikController = new StatistikController();
                    var forsaljning = statistikController.GetForsaljningRestaurang(restaurang.RestaurangID, igår, igår);
                    var mestSalda = statistikController.GetMestSaldaRatter(restaurang.RestaurangID, igår, igår, 10);
                    var minstSalda = statistikController.GetMinstSaldaRatter(restaurang.RestaurangID, igår, igår, 10);
                    var servitorer = statistikController.GetForsaljningPerServitor(restaurang.RestaurangID, igår, igår);
                    var bokningar = statistikController.GetBokningsStatistik(restaurang.RestaurangID, igår, igår);

                    // Generera PDF
                    string pdfPath = _pdfService.GenerateRestaurangchefRapport(
                        restaurang.Restaurangnamn,
                        igår,
                        igår,
                        forsaljning,
                        mestSalda,
                        minstSalda,
                        servitorer,
                        bokningar);

                    // Generera bokföringsfil
                    string bokforingPath = _bokföringsService.GeneraBokföringRestaurang(restaurang.RestaurangID, igår);

                    // Skicka PDF till restaurangchef
                    await _mailService.SendStatistikRapportAsync(
                        "restaurangchef@restonation.se",
                        restaurang.Restaurangnamn,
                        $"{igår:yyyy-MM-dd}",
                        pdfPath);

                    // Skicka bokföringsfil till ekonomi
                    await _mailService.SendBokforingsfilAsync(
                        "ekonomi@restonation.se",
                        igår,
                        bokforingPath);

                    // Logga
                    _loggService.LoggaHandelse(0, "SchemaController", "Daglig rapport skickad",
                        $"Restaurang: {restaurang.Restaurangnamn}, Datum: {igår:yyyy-MM-dd}");
                }
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(0, "SchemaController", "FEL vid daglig rapport", ex.Message);
            }
        }

        private async Task GenereraVeckovisKundExport()
        {
            try
            {
                // Exportera alla kunder
                string kundlistaFil = _kundExportService.ExporteraAllaKunder();

                // Skicka till marknadsavdelningen
                bool success = await _mailService.SendEmailWithAttachmentAsync(
                    "marknad@restonation.se",
                    $"Veckovis kundlista - {DateTime.Now:yyyy-MM-dd}",
                    "<h2>Veckovis kundexport</h2><p>Bifogad fil innehåller alla kunder med lojalitetsnivå, region och hemmarestaurang.</p>",
                    kundlistaFil);

                // Logga
                if (success)
                {
                    _loggService.LoggaHandelse(0, "SchemaController", "Veckovis kundexport skickad",
                        $"Fil: {kundlistaFil}");
                }
                else
                {
                    _loggService.LoggaHandelse(0, "SchemaController", "FEL vid kundexport",
                        $"Kunde inte skicka mail. {_mailService.LastError}");
                }
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(0, "SchemaController", "FEL vid kundexport", ex.Message);
            }
        }


        public void Dispose()
        {
            foreach (var timer in _timers)
            {
                timer.Stop();
                timer.Dispose();
            }
        }
    }
}
