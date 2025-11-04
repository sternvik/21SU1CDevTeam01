using AffärsLager.DTOs;
using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Services
{
    /// <summary>
    /// StatistikService - Beräknar och hämtar statistik för restauranger
    /// Ansvarar för försäljningsstatistik, personalstatistik, menystatistik och bokföringsdata
    /// Används av VD och Restaurangchefer för att följa upp verksamheten
    /// </summary>
    public class StatistikService
    {
        private readonly UnitOfWork _unitOfWork;

        public StatistikService()
        {
            _unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Hämtar personalstatistik för en restaurang under en tidsperiod
        /// Visar hur många bokningar varje anställd hanterat, försäljning, dricks etc.
        /// Används för att följa upp personalens prestationer
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <param name="franDatum">Startdatum för perioden</param>
        /// <param name="tillDatum">Slutdatum för perioden</param>
        /// <returns>Lista med statistik för varje anställd, sorterad på antal bokningar</returns>
        public List<PersonalStatistikDto> HamtaPersonalStatistik(int restaurangId, DateTime franDatum, DateTime tillDatum)
        {
            // Uppdatera context för att få färsk data från databasen
            _unitOfWork.RefreshContext();

            // Hämta alla bokningar för restaurangen under perioden (exklusive avbokade)
            var bokningar = _unitOfWork.BokningRepository.GetAll()
                .Where(b => b.RestaurangID == restaurangId &&
                           b.Datum.Date >= franDatum.Date &&
                           b.Datum.Date <= tillDatum.Date &&
                           b.Status != "Avbokad")
                .ToList();

            // Hämta alla beställningar för restaurangen under perioden
            // Dessa används för att räkna ut försäljning och dricks per personal
            var bestallningar = _unitOfWork.BestallningRepository.GetAll()
                .Where(b => b.RestaurangID == restaurangId &&
                           b.Datum >= franDatum &&
                           b.Datum <= tillDatum)
                .ToList();

            // Gruppera bokningarna per personal (AnvandarID) och skapa statistik
            var personalStatistik = bokningar
                .GroupBy(b => b.AnvandarID)  // Gruppera på personal
                .Select(g => new PersonalStatistikDto
                {
                    AnvandarID = g.Key ?? 0,
                    PersonalNamn = _unitOfWork.AnvandareRepository
                        .FirstOrDefault(a => a.AnvandarID == g.Key)?.Namn ?? "Okänd",
                    RestaurangID = restaurangId,
                    RestaurangNamn = _unitOfWork.RestaurangRepository
                        .FirstOrDefault(r => r.RestaurangID == restaurangId)?.Restaurangnamn ?? "",
                    AntalBokningar = g.Count(),  // Hur många bokningar har personalen hanterat?
                    AntalBordHanterade = g.Select(b => b.BordID).Distinct().Count(),  // Hur många unika bord?
                    TotaltAntalGaster = g.Sum(b => b.AntalGaster),  // Totalt antal gäster personalen servert
                    // Summera försäljning från alla beställningar som personalen hanterat
                    TotalForsaljning = bestallningar
                        .Where(best => best.AnvandarID == g.Key)
                        .Sum(best => best.TotalSumma),
                    // Summera dricks från alla beställningar som personalen hanterat
                    TotalDricks = bestallningar
                        .Where(best => best.AnvandarID == g.Key)
                        .Sum(best => best.Dricks)
                })
                .OrderByDescending(p => p.AntalBokningar)  // Sortera så att den med flest bokningar kommer först
                .ToList();

            return personalStatistik;
        }

        /// <summary>
        /// Hämtar menystatistik för en restaurang under en tidsperiod
        /// Visar vilka rätter som säljer bäst, hur många som sålts, försäljning etc.
        /// Används för att se vilka rätter som är populära och vilka som inte säljer
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <param name="franDatum">Startdatum för perioden</param>
        /// <param name="tillDatum">Slutdatum för perioden</param>
        /// <returns>Lista med statistik för varje menyvaror, sorterad på antal sålda</returns>
        public List<MenyStatistikDto> HamtaMenyStatistik(int restaurangId, DateTime franDatum, DateTime tillDatum)
        {
            // Uppdatera context för att få färsk data
            _unitOfWork.RefreshContext();

            // Hämta alla beställningsrader för restaurangen under perioden
            // Varje rad representerar en rätt som beställts (t.ex. "2x Pizza Margherita")
            var bestallningsRader = _unitOfWork.BestallningsRadRepository.GetAll()
                .Where(br => br.Bestallning.RestaurangID == restaurangId &&
                            br.Bestallning.Datum >= franDatum &&
                            br.Bestallning.Datum <= tillDatum)
                .ToList();

            // Gruppera raderna per menyvaror och räkna statistik
            var menyStatistik = bestallningsRader
                .GroupBy(br => br.MenyID)  // Gruppera på vilken rätt det är
                .Select(g =>
                {
                    // Hämta menyinformation från databasen
                    var meny = _unitOfWork.MenyRepository.FirstOrDefault(m => m.MenyID == g.Key);
                    return new MenyStatistikDto
                    {
                        MenyID = g.Key,
                        Rattnamn = meny?.Rattnamn ?? "Okänd",
                        Kategori = meny?.Kategori ?? "Okänd",
                        AntalSalda = g.Sum(br => br.Antal),  // Summera alla beställda antal (2+3+1 = 6 sålda)
                        TotalForsaljning = g.Sum(br => br.Pris * br.Antal),  // Pris * Antal för varje rad
                        GenomsnittsPris = meny?.Pris ?? 0,
                        ArGrundmeny = meny?.ArGrundmeny ?? false,
                        RestaurangID = restaurangId
                    };
                })
                .OrderByDescending(m => m.AntalSalda)  // Sortera så mest sålda rätten kommer först
                .ToList();

            return menyStatistik;
        }

        /// <summary>
        /// Hämtar en komplett försäljningssammanfattning för en restaurang
        /// Detta är en "master-rapport" som samlar all statistik på ett ställe
        /// Inkluderar: total försäljning, dricks, mat/dryck-uppdelning, bäst/sämst säljande rätter, personalstatistik
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <param name="franDatum">Startdatum för perioden</param>
        /// <param name="tillDatum">Slutdatum för perioden</param>
        /// <returns>Komplett försäljningssammanfattning med all statistik</returns>
        public ForsaljningsSummaryDto HamtaForsaljningsSummary(int restaurangId, DateTime franDatum, DateTime tillDatum)
        {
            _unitOfWork.RefreshContext();

            var restaurang = _unitOfWork.RestaurangRepository.FirstOrDefault(r => r.RestaurangID == restaurangId);

            // VIKTIGT: Jämför bara DATE, inte tid!
            // Detta för att undvika problem med tidskomponenten (12:00 vs 14:30 etc)
            var bokningar = _unitOfWork.BokningRepository.GetAll()
                .Where(b => b.RestaurangID == restaurangId &&
                           b.Datum.Date >= franDatum.Date &&
                           b.Datum.Date <= tillDatum.Date &&
                           b.Status != "Avbokad")
                .ToList();

            var bestallningar = _unitOfWork.BestallningRepository.GetAll()
                .Where(b => b.RestaurangID == restaurangId &&
                           b.Datum.Date >= franDatum.Date &&
                           b.Datum.Date <= tillDatum.Date)
                .ToList();

            // Hämta menystatistik för att kunna dela upp Mat/Dryck
            var menyStatistik = HamtaMenyStatistik(restaurangId, franDatum, tillDatum);

            return new ForsaljningsSummaryDto
            {
                RestaurangID = restaurangId,
                RestaurangNamn = restaurang?.Restaurangnamn ?? "",
                FranDatum = franDatum,
                TillDatum = tillDatum,
                TotalForsaljning = bestallningar.Sum(b => b.TotalSumma),  // Total försäljning
                TotalDricks = bestallningar.Sum(b => b.Dricks),  // Total dricks
                TotaltAntalBestallningar = bestallningar.Count,  // Antal beställningar
                TotaltAntalBokningar = bokningar.Count,  // Antal bokningar
                TotaltAntalGaster = bokningar.Sum(b => b.AntalGaster),  // Antal gäster som besökt restaurangen
                // Dela upp försäljning på Mat (viktigt för bokföring - olika moms)
                MatForsaljning = menyStatistik
                    .Where(m => m.Kategori.ToLower().Contains("mat"))
                    .Sum(m => m.TotalForsaljning),
                // Dela upp försäljning på Dryck (inkl. alkohol - viktigt för bokföring)
                DryckForsaljning = menyStatistik
                    .Where(m => m.Kategori.ToLower().Contains("dryck") ||
                               m.Kategori.ToLower().Contains("alkohol") ||
                               m.Kategori.ToLower().Contains("öl") ||
                               m.Kategori.ToLower().Contains("vin") ||
                               m.Kategori.ToLower().Contains("sprit"))
                    .Sum(m => m.TotalForsaljning),
                MestSaldaRatter = menyStatistik.Take(10).ToList(),  // Top 10 bäst säljande rätter
                MinstSaldaRatter = menyStatistik.OrderBy(m => m.AntalSalda).Take(10).ToList(),  // 10 sämst säljande
                PersonalStatistik = HamtaPersonalStatistik(restaurangId, franDatum, tillDatum)  // All personalstatistik
            };
        }

        /// <summary>
        /// Hämtar statistik för grundmenyn över ALLA restauranger
        /// Visar hur populära grundmenyns rätter är totalt, och per restaurang
        /// Används av VD för att se vilka grundmenyrätter som fungerar bra/dåligt
        /// </summary>
        /// <param name="franDatum">Startdatum för perioden</param>
        /// <param name="tillDatum">Slutdatum för perioden</param>
        /// <returns>Lista med statistik för varje grundmenyrätt</returns>
        public List<GrundmenyStatistikDto> HamtaGrundmenyStatistik(DateTime franDatum, DateTime tillDatum)
        {
            _unitOfWork.RefreshContext();

            // Hämta alla rätter som tillhör grundmenyn (finns på alla restauranger)
            var grundmenyer = _unitOfWork.MenyRepository.GetAll()
                .Where(m => m.ArGrundmeny)
                .ToList();

            // Hämta alla beställningsrader för grundmenyrätter under perioden
            var bestallningsRader = _unitOfWork.BestallningsRadRepository.GetAll()
                .Where(br => br.Bestallning.Datum >= franDatum &&
                            br.Bestallning.Datum <= tillDatum &&
                            br.Meny.ArGrundmeny)
                .ToList();

            // För varje grundmenyrätt, räkna ut total försäljning och per-restaurang
            var statistik = grundmenyer.Select(meny =>
            {
                // Filtrera ut alla beställningsrader för just denna rätt
                var raderForMeny = bestallningsRader.Where(br => br.MenyID == meny.MenyID).ToList();

                return new GrundmenyStatistikDto
                {
                    MenyID = meny.MenyID,
                    Rattnamn = meny.Rattnamn,
                    Kategori = meny.Kategori,
                    TotaltAntalSalda = raderForMeny.Sum(br => br.Antal),  // Totalt över alla restauranger
                    TotalForsaljning = raderForMeny.Sum(br => br.Pris * br.Antal),
                    // Skapa en dictionary som visar försäljning per restaurang
                    // T.ex. "Pizza Napoli": { "Malmö": 45, "Stockholm": 67, "Göteborg": 23 }
                    ForsaljningPerRestaurang = raderForMeny
                        .GroupBy(br => br.Bestallning.RestaurangID)
                        .ToDictionary(
                            g => _unitOfWork.RestaurangRepository
                                .FirstOrDefault(r => r.RestaurangID == g.Key)?.Restaurangnamn ?? $"Restaurang {g.Key}",
                            g => g.Sum(br => br.Antal)
                        )
                };
            })
            .OrderByDescending(s => s.TotaltAntalSalda)  // Sortera på mest sålda först
            .ToList();

            return statistik;
        }

        /// <summary>
        /// Genererar bokföringsdata för en specifik dag
        /// Samlar ihop all försäljning för dagen som ska bokföras i ekonomisystemet
        /// OBS: För mer detaljerad bokföring (Mat/Alkohol separat), använd BokföringsService istället
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <param name="datum">Datumet att generera bokföring för</param>
        /// <returns>Bokföringsdata för dagen (dagssumma, transaktioner, dricks etc.)</returns>
        public BokforingDto GenereraBokforing(int restaurangId, DateTime datum)
        {
            _unitOfWork.RefreshContext();

            var restaurang = _unitOfWork.RestaurangRepository.FirstOrDefault(r => r.RestaurangID == restaurangId);

            // Hämta alla BETALDA beställningar för denna dag
            // Endast betalda beställningar ska bokföras
            var bestallningar = _unitOfWork.BestallningRepository.GetAll()
                .Where(b => b.RestaurangID == restaurangId &&
                           b.Datum.Date == datum.Date &&
                           b.Betald)
                .ToList();

            return new BokforingDto
            {
                Datum = datum,
                RestaurangID = restaurangId,
                RestaurangNamn = restaurang?.Restaurangnamn ?? "",
                Dagssumma = bestallningar.Sum(b => b.TotalSumma),  // Total dagsomsättning
                AntalTransaktioner = bestallningar.Count,  // Antal beställningar/kvitton
                Dricks = bestallningar.Sum(b => b.Dricks),  // Total dricks för dagen
                KontantBetalningar = 0, // OBS: Skulle behöva betalningsmetod-info från systemet
                KortBetalningar = bestallningar.Sum(b => b.TotalSumma),  // Antar att allt är kort för nu
                LojalitetsPoangAnvanda = 0 // OBS: Skulle behöva info från lojalitetstransaktioner
            };
        }
    }
}
