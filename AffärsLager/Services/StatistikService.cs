using AffärsLager.DTOs;
using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Services
{
    /// <summary>
    /// Service för att beräkna och hämta statistik
    /// </summary>
    public class StatistikService
    {
        private readonly UnitOfWork _unitOfWork;

        public StatistikService()
        {
            _unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Hämta personalstatistik för en restaurang
        /// </summary>
        public List<PersonalStatistikDto> HamtaPersonalStatistik(int restaurangId, DateTime franDatum, DateTime tillDatum)
        {
            _unitOfWork.RefreshContext();

            var bokningar = _unitOfWork.BokningRepository.GetAll()
                .Where(b => b.RestaurangID == restaurangId &&
                           b.Datum.Date >= franDatum.Date &&
                           b.Datum.Date <= tillDatum.Date &&
                           b.Status != "Avbokad")
                .ToList();

            var bestallningar = _unitOfWork.BestallningRepository.GetAll()
                .Where(b => b.RestaurangID == restaurangId &&
                           b.Datum >= franDatum &&
                           b.Datum <= tillDatum)
                .ToList();

            var personalStatistik = bokningar
                .GroupBy(b => b.AnvandarID)
                .Select(g => new PersonalStatistikDto
                {
                    AnvandarID = g.Key ?? 0,
                    PersonalNamn = _unitOfWork.AnvandareRepository
                        .FirstOrDefault(a => a.AnvandarID == g.Key)?.Namn ?? "Okänd",
                    RestaurangID = restaurangId,
                    RestaurangNamn = _unitOfWork.RestaurangRepository
                        .FirstOrDefault(r => r.RestaurangID == restaurangId)?.Restaurangnamn ?? "",
                    AntalBokningar = g.Count(),
                    AntalBordHanterade = g.Select(b => b.BordID).Distinct().Count(),
                    TotaltAntalGaster = g.Sum(b => b.AntalGaster),
                    TotalForsaljning = bestallningar
                        .Where(best => best.AnvandarID == g.Key)
                        .Sum(best => best.TotalSumma)
                })
                .OrderByDescending(p => p.AntalBokningar)
                .ToList();

            return personalStatistik;
        }

        /// <summary>
        /// Hämta menystatistik för en restaurang
        /// </summary>
        public List<MenyStatistikDto> HamtaMenyStatistik(int restaurangId, DateTime franDatum, DateTime tillDatum)
        {
            _unitOfWork.RefreshContext();

            var bestallningsRader = _unitOfWork.BestallningsRadRepository.GetAll()
                .Where(br => br.Bestallning.RestaurangID == restaurangId &&
                            br.Bestallning.Datum >= franDatum &&
                            br.Bestallning.Datum <= tillDatum)
                .ToList();

            var menyStatistik = bestallningsRader
                .GroupBy(br => br.MenyID)
                .Select(g =>
                {
                    var meny = _unitOfWork.MenyRepository.FirstOrDefault(m => m.MenyID == g.Key);
                    return new MenyStatistikDto
                    {
                        MenyID = g.Key,
                        Rattnamn = meny?.Rattnamn ?? "Okänd",
                        Kategori = meny?.Kategori ?? "Okänd",
                        AntalSalda = g.Sum(br => br.Antal),
                        TotalForsaljning = g.Sum(br => br.Pris * br.Antal),
                        GenomsnittsPris = meny?.Pris ?? 0,
                        ArGrundmeny = meny?.ArGrundmeny ?? false,
                        RestaurangID = restaurangId
                    };
                })
                .OrderByDescending(m => m.AntalSalda)
                .ToList();

            return menyStatistik;
        }

        /// <summary>
        /// Hämta försäljningssammanfattning för en restaurang
        /// </summary>
        public ForsaljningsSummaryDto HamtaForsaljningsSummary(int restaurangId, DateTime franDatum, DateTime tillDatum)
        {
            _unitOfWork.RefreshContext();

            var restaurang = _unitOfWork.RestaurangRepository.FirstOrDefault(r => r.RestaurangID == restaurangId);
    
            // VIKTIGT: Jämför bara DATE, inte tid!
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

            var menyStatistik = HamtaMenyStatistik(restaurangId, franDatum, tillDatum);

            return new ForsaljningsSummaryDto
            {
                RestaurangID = restaurangId,
                RestaurangNamn = restaurang?.Restaurangnamn ?? "",
                FranDatum = franDatum,
                TillDatum = tillDatum,
                TotalForsaljning = bestallningar.Sum(b => b.TotalSumma),
                TotaltAntalBestallningar = bestallningar.Count,
                TotaltAntalBokningar = bokningar.Count,
                TotaltAntalGaster = bokningar.Sum(b => b.AntalGaster),
                MatForsaljning = menyStatistik
                    .Where(m => m.Kategori.ToLower().Contains("carte") || 
                               m.Kategori.ToLower().Contains("lunch"))
                    .Sum(m => m.TotalForsaljning),
                DryckForsaljning = menyStatistik
                    .Where(m => m.Kategori.ToLower().Contains("dryck"))
                    .Sum(m => m.TotalForsaljning),
                MestSaldaRatter = menyStatistik.Take(10).ToList(),
                MinstSaldaRatter = menyStatistik.OrderBy(m => m.AntalSalda).Take(10).ToList(),
                PersonalStatistik = HamtaPersonalStatistik(restaurangId, franDatum, tillDatum)
            };
        }

        /// <summary>
        /// Hämta grundmenystatistik (alla restauranger)
        /// </summary>
        public List<GrundmenyStatistikDto> HamtaGrundmenyStatistik(DateTime franDatum, DateTime tillDatum)
        {
            _unitOfWork.RefreshContext();

            var grundmenyer = _unitOfWork.MenyRepository.GetAll()
                .Where(m => m.ArGrundmeny)
                .ToList();

            var bestallningsRader = _unitOfWork.BestallningsRadRepository.GetAll()
                .Where(br => br.Bestallning.Datum >= franDatum &&
                            br.Bestallning.Datum <= tillDatum &&
                            br.Meny.ArGrundmeny)
                .ToList();

            var statistik = grundmenyer.Select(meny =>
            {
                var raderForMeny = bestallningsRader.Where(br => br.MenyID == meny.MenyID).ToList();
                
                return new GrundmenyStatistikDto
                {
                    MenyID = meny.MenyID,
                    Rattnamn = meny.Rattnamn,
                    Kategori = meny.Kategori,
                    TotaltAntalSalda = raderForMeny.Sum(br => br.Antal),
                    TotalForsaljning = raderForMeny.Sum(br => br.Pris * br.Antal),
                    ForsaljningPerRestaurang = raderForMeny
                        .GroupBy(br => br.Bestallning.RestaurangID)
                        .ToDictionary(
                            g => _unitOfWork.RestaurangRepository
                                .FirstOrDefault(r => r.RestaurangID == g.Key)?.Restaurangnamn ?? $"Restaurang {g.Key}",
                            g => g.Sum(br => br.Antal)
                        )
                };
            })
            .OrderByDescending(s => s.TotaltAntalSalda)
            .ToList();

            return statistik;
        }

        /// <summary>
        /// Generera bokföringsdata för en dag
        /// </summary>
        public BokforingDto GenereraBokforing(int restaurangId, DateTime datum)
        {
            _unitOfWork.RefreshContext();

            var restaurang = _unitOfWork.RestaurangRepository.FirstOrDefault(r => r.RestaurangID == restaurangId);
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
                Dagssumma = bestallningar.Sum(b => b.TotalSumma),
                AntalTransaktioner = bestallningar.Count,
                Dricks = 0, // Skulle behöva hämtas från transaktionstabellen om vi sparar dricks
                KontantBetalningar = 0, // Skulle behöva betalningsmetod-info
                KortBetalningar = bestallningar.Sum(b => b.TotalSumma),
                LojalitetsPoangAnvanda = 0 // Skulle behöva info från lojalitetstabellen
            };
        }
    }
}
