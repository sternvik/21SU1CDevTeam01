using AffärsLager.Services;
using System;
using System.Collections.Generic;
using Timer = System.Timers.Timer;

namespace AffärsLager.Controllers
{
    public class SchemaController : IDisposable
    {
        private readonly List<Timer> _timers = new();
        private readonly StatistikService _statistikService;
        private readonly PdfGeneratorService _pdfService;
        private readonly EmailService _emailService;
        private readonly BokforingService _bokforingService;
        private readonly KundExportService _kundExportService;
        private readonly LoggService _loggService;

        public SchemaController()
        {
            _statistikService = new StatistikService();
            _pdfService = new PdfGeneratorService();
            _emailService = new EmailService();
            _bokforingService = new BokforingService();
            _kundExportService = new KundExportService();
            _loggService = new LoggService();
        }

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

        private async System.Threading.Tasks.Task GenereraDagligRapport()
        {
            try
            {
                // För varje restaurang (1-18)
                for (int restId = 1; restId <= 18; restId++)
                {
                    var igår = DateTime.Today.AddDays(-1);

                    // Generera statistik
                    var summary = _statistikService.HamtaForsaljningsSummary(restId, igår, igår);

                    // Generera PDF
                    var pdfFil = _pdfService.GeneraStatistikRapport(summary);

                    // Generera bokföringsfil
                    var csvFil = _bokforingService.GenereraCsvFil(restId, igår);

                    // Skicka emails
                    await _emailService.SkickaEmail(
                        "restaurangchef@restonation.dk",
                        $"Daglig statistik - {summary.RestaurangNamn}",
                        $"<h2>Daglig rapport för {igår:yyyy-MM-dd}</h2><p>Total försäljning: {summary.TotalForsaljning:C}</p>",
                        new List<string> { pdfFil }
                    );

                    await _emailService.SkickaEmail(
                        "ekonomi@restonation.dk",
                        $"Bokföring - {summary.RestaurangNamn}",
                        $"<p>Bokföringsfil för {igår:yyyy-MM-dd}</p>",
                        new List<string> { csvFil }
                    );

                    // Logga
                    _loggService.LoggaHandelse(0, "SchemaController", "Daglig rapport skickad", $"Restaurang: {restId}");
                }
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(0, "SchemaController", "FEL vid daglig rapport", ex.Message);
            }
        }

        private async System.Threading.Tasks.Task GenereraVeckovisKundExport()
        {
            try
            {
                // Exportera alla kunder
                var kundlistaFil = _kundExportService.ExporteraAllaKunder();

                // Skicka till marknadsavdelningen
                await _emailService.SkickaEmail(
                    "marknad@restonation.se",
                    $"Veckovis kundlista - {DateTime.Now:yyyy-MM-dd}",
                    "<h2>Veckovis kundexport</h2><p>Bifogad fil innehåller alla kunder med lojalitetsnivå, region och hemmarestaurang.</p>",
                    new List<string> { kundlistaFil }
                );

                // Logga
                _loggService.LoggaHandelse(0, "SchemaController", "Veckovis kundexport skickad", $"Fil: {kundlistaFil}");
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