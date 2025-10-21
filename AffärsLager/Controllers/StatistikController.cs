using AffärsLager.DTOs;
using AffärsLager.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class StatistikController
    {
        private readonly StatistikService _statistikService;
        private readonly LoggService _loggService;

        public StatistikController()
        {
            _statistikService = new StatistikService();
            _loggService = new LoggService();
        }

        public List<PersonalStatistikDto> HamtaPersonalStatistik(int restaurangId, DateTime franDatum, DateTime tillDatum, int anvandarId)
        {
            try
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "Hämtade personalstatistik",
                    $"Restaurang: {restaurangId}, Period: {franDatum:yyyy-MM-dd} - {tillDatum:yyyy-MM-dd}");
                return _statistikService.HamtaPersonalStatistik(restaurangId, franDatum, tillDatum);
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "FEL vid hämtning av personalstatistik", ex.Message);
                throw;
            }
        }

        public List<MenyStatistikDto> HamtaMenyStatistik(int restaurangId, DateTime franDatum, DateTime tillDatum, int anvandarId)
        {
            try
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "Hämtade menystatistik",
                    $"Restaurang: {restaurangId}, Period: {franDatum:yyyy-MM-dd} - {tillDatum:yyyy-MM-dd}");
                return _statistikService.HamtaMenyStatistik(restaurangId, franDatum, tillDatum);
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "FEL vid hämtning av menystatistik", ex.Message);
                throw;
            }
        }

        public ForsaljningsSummaryDto HamtaForsaljningsSummary(int restaurangId, DateTime franDatum, DateTime tillDatum, int anvandarId)
        {
            try
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "Hämtade försäljningssammanfattning",
                    $"Restaurang: {restaurangId}, Period: {franDatum:yyyy-MM-dd} - {tillDatum:yyyy-MM-dd}");
                return _statistikService.HamtaForsaljningsSummary(restaurangId, franDatum, tillDatum);
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "FEL vid hämtning av försäljningssammanfattning", ex.Message);
                throw;
            }
        }

        public List<GrundmenyStatistikDto> HamtaGrundmenyStatistik(DateTime franDatum, DateTime tillDatum, int anvandarId)
        {
            try
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "Hämtade grundmenystatistik (alla restauranger)",
                    $"Period: {franDatum:yyyy-MM-dd} - {tillDatum:yyyy-MM-dd}");
                return _statistikService.HamtaGrundmenyStatistik(franDatum, tillDatum);
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "FEL vid hämtning av grundmenystatistik", ex.Message);
                throw;
            }
        }

        public ForsaljningsSummaryDto HamtaKoncernStatistik(DateTime franDatum, DateTime tillDatum, int anvandarId)
        {
            try
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "Hämtade koncernstatistik",
                    $"Period: {franDatum:yyyy-MM-dd} - {tillDatum:yyyy-MM-dd}");

                var koncernSummary = new ForsaljningsSummaryDto
                {
                    RestaurangID = 0,
                    RestaurangNamn = "Hela Koncernen",
                    FranDatum = franDatum,
                    TillDatum = tillDatum,
                    MestSaldaRatter = new List<MenyStatistikDto>(),
                    MinstSaldaRatter = new List<MenyStatistikDto>(),
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
                        koncernSummary.PersonalStatistik.AddRange(restSummary.PersonalStatistik);
                    }
                    catch { }
                }

                var grundmenyStats = _statistikService.HamtaGrundmenyStatistik(franDatum, tillDatum);
                foreach (var menystat in grundmenyStats)
                {
                    koncernSummary.MestSaldaRatter.Add(new MenyStatistikDto
                    {
                        MenyID = menystat.MenyID,
                        Rattnamn = menystat.Rattnamn,
                        Kategori = menystat.Kategori,
                        AntalSalda = menystat.TotaltAntalSalda,
                        TotalForsaljning = menystat.TotalForsaljning,
                        ArGrundmeny = true
                    });
                }

                koncernSummary.MestSaldaRatter = koncernSummary.MestSaldaRatter
                    .OrderByDescending(m => m.AntalSalda).Take(10).ToList();

                return koncernSummary;
            }
            catch (Exception ex)
            {
                _loggService.LoggaHandelse(anvandarId, "StatistikController", "FEL vid hämtning av koncernstatistik", ex.Message);
                throw;
            }
        }
    }
}