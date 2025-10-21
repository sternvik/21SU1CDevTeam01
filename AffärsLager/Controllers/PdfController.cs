using AffärsLager.DTOs;
using AffärsLager.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class PdfController
    {
        private readonly PdfGeneratorService _pdfService;
        private readonly StatistikService _statistikService;
        private readonly LoggService _loggService;
        private readonly EmailService _emailService;

        public PdfController()
        {
            _pdfService = new PdfGeneratorService();
            _statistikService = new StatistikService();
            _loggService = new LoggService();
            _emailService = new EmailService();
        }

        public string GeneraStatistikRapport(int restaurangId, DateTime franDatum, DateTime tillDatum, int anvandarId)
        {
            try
            {
                var summary = _statistikService.HamtaForsaljningsSummary(restaurangId, franDatum, tillDatum);
                var pdfFil = _pdfService.GeneraStatistikRapport(summary);
                _loggService.LoggaHandelse(anvandarId, "PdfController", "Genererade PDF-rapport", $"Fil: {pdfFil}");
                return pdfFil;
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(anvandarId, "PdfController", "FEL vid PDF-generering", ex.Message);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<bool> GeneraOchSkickaPdfRapport(
            int restaurangId, DateTime franDatum, DateTime tillDatum, string emailMottagare, int anvandarId)
        {
            try
            {
                var summary = _statistikService.HamtaForsaljningsSummary(restaurangId, franDatum, tillDatum);
                var pdfFil = _pdfService.GeneraStatistikRapport(summary);

                var skickad = await _emailService.SkickaEmail(
                    emailMottagare,
                    $"Statistikrapport - {summary.RestaurangNamn}",
                    $"<h2>Statistikrapport</h2><p>Period: {franDatum:yyyy-MM-dd} till {tillDatum:yyyy-MM-dd}</p><p>Total försäljning: {summary.TotalForsaljning:C}</p>",
                    new List<string> { pdfFil }
                );

                if (skickad)
                    _loggService.LoggaHandelse(anvandarId, "PdfController", "Skickade PDF-rapport via email", $"Till: {emailMottagare}");
                else
                    _loggService.LoggaHandelse(anvandarId, "PdfController", "FEL vid email-sändning", $"Kunde inte skicka till: {emailMottagare}");

                return skickad;
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(anvandarId, "PdfController", "FEL vid PDF-generering och email", ex.Message);
                throw;
            }
        }

        public string GeneraKoncernRapport(DateTime franDatum, DateTime tillDatum, int anvandarId)
        {
            try
            {
                var koncernSummary = new ForsaljningsSummaryDto
                {
                    RestaurangID = 0,
                    RestaurangNamn = "RestoNation Koncernen",
                    FranDatum = franDatum,
                    TillDatum = tillDatum,
                    MestSaldaRatter = new List<MenyStatistikDto>(),
                    PersonalStatistik = new List<PersonalStatistikDto>()
                };

                for (int restId = 1; restId <= 18; restId++)
                {
                    try
                    {
                        var restSummary = _statistikService.HamtaForsaljningsSummary(restId, franDatum, tillDatum);
                        koncernSummary.TotalForsaljning += restSummary.TotalForsaljning;
                        koncernSummary.TotaltAntalBestallningar += restSummary.TotaltAntalBestallningar;
                        koncernSummary.TotaltAntalBokningar += restSummary.TotaltAntalBokningar;
                        koncernSummary.TotaltAntalGaster += restSummary.TotaltAntalGaster;
                        koncernSummary.MatForsaljning += restSummary.MatForsaljning;
                        koncernSummary.DryckForsaljning += restSummary.DryckForsaljning;
                    }
                    catch { }
                }

                var pdfFil = _pdfService.GeneraStatistikRapport(koncernSummary);
                _loggService.LoggaHandelse(anvandarId, "PdfController", "Genererade koncern-PDF-rapport", $"Fil: {pdfFil}");
                return pdfFil;
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(anvandarId, "PdfController", "FEL vid koncern-PDF-generering", ex.Message);
                throw;
            }
        }
    }
}