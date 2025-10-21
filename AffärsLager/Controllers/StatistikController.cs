using EntitetsLager;
using DataLager;
using Microsoft.EntityFrameworkCore;

namespace AffärsLager.Controllers
{
    public class StatistikController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        /// <summary>
        /// Hjälpmetod för att avgöra om en kategori är alkohol/dryck
        /// </summary>
        private bool IsAlkoholKategori(string kategori)
        {
            if (string.IsNullOrEmpty(kategori)) return false;

            var lowerKategori = kategori.ToLower();

            // Exclude non-alcoholic drinks
            if (lowerKategori.Contains("alkoholfri")) return false;

            // Include alcoholic categories
            return lowerKategori.Contains("alkoholhaltig") ||
                   lowerKategori.Contains("öl") ||
                   lowerKategori.Contains("vin") ||
                   lowerKategori.Contains("sprit") ||
                   (lowerKategori.Contains("alkohol") && !lowerKategori.Contains("fri"));
        }

        #region Restaurangchef Statistik

        /// <summary>
        /// Hämtar försäljningsstatistik för en specifik restaurang
        /// </summary>
        public ForsaljningsStatistik GetForsaljningRestaurang(int restaurangId, DateTime startDatum, DateTime slutDatum)
        {
            var bestallningar = _unitOfWork.BestallningRepository.GetQuery()
                .Where(b => b.RestaurangID == restaurangId && b.Datum >= startDatum && b.Datum <= slutDatum)
                .Include(b => b.BestallningsRader)
                .ThenInclude(br => br.Meny)
                .ToList();

            // Beräkna mat och alkohol baserat på meny-kategori
            decimal matSumma = 0;
            decimal alkoholSumma = 0;

            foreach (var bestallning in bestallningar)
            {
                foreach (var rad in bestallning.BestallningsRader)
                {
                    if (IsAlkoholKategori(rad.Meny.Kategori))
                    {
                        alkoholSumma += rad.Summa;
                    }
                    else
                    {
                        matSumma += rad.Summa;
                    }
                }
            }

            return new ForsaljningsStatistik
            {
                TotalForsaljning = bestallningar.Sum(b => b.TotalSumma),
                MatSumma = matSumma,
                AlkoholSumma = alkoholSumma,
                AntalTransaktioner = bestallningar.Count
            };
        }

        /// <summary>
        /// Hämtar mest sålda rätter för en restaurang
        /// </summary>
        public List<RattStatistik> GetMestSaldaRatter(int restaurangId, DateTime startDatum, DateTime slutDatum, int antal = 10)
        {
            var bestallningar = _unitOfWork.BestallningRepository.GetQuery()
                .Where(b => b.RestaurangID == restaurangId && b.Datum >= startDatum && b.Datum <= slutDatum)
                .Include(b => b.BestallningsRader)
                .ThenInclude(br => br.Meny)
                .ToList();

            var rattStatistik = bestallningar
                .SelectMany(b => b.BestallningsRader)
                .GroupBy(br => new { br.MenyID, br.Meny.Rattnamn, br.Meny.Kategori })
                .Select(g => new RattStatistik
                {
                    MenyID = g.Key.MenyID,
                    Rattnamn = g.Key.Rattnamn,
                    Kategori = g.Key.Kategori,
                    AntalSalda = g.Sum(br => br.Antal),
                    TotalForsaljning = g.Sum(br => br.Summa)
                })
                .OrderByDescending(r => r.AntalSalda)
                .Take(antal)
                .ToList();

            return rattStatistik;
        }

        /// <summary>
        /// Hämtar minst sålda rätter för en restaurang
        /// </summary>
        public List<RattStatistik> GetMinstSaldaRatter(int restaurangId, DateTime startDatum, DateTime slutDatum, int antal = 10)
        {
            var bestallningar = _unitOfWork.BestallningRepository.GetQuery()
                .Where(b => b.RestaurangID == restaurangId && b.Datum >= startDatum && b.Datum <= slutDatum)
                .Include(b => b.BestallningsRader)
                .ThenInclude(br => br.Meny)
                .ToList();

            var rattStatistik = bestallningar
                .SelectMany(b => b.BestallningsRader)
                .GroupBy(br => new { br.MenyID, br.Meny.Rattnamn, br.Meny.Kategori })
                .Select(g => new RattStatistik
                {
                    MenyID = g.Key.MenyID,
                    Rattnamn = g.Key.Rattnamn,
                    Kategori = g.Key.Kategori,
                    AntalSalda = g.Sum(br => br.Antal),
                    TotalForsaljning = g.Sum(br => br.Summa)
                })
                .OrderBy(r => r.AntalSalda)
                .Take(antal)
                .ToList();

            return rattStatistik;
        }

        /// <summary>
        /// Hämtar försäljning per servitör för en restaurang
        /// </summary>
        public List<ServitorStatistik> GetForsaljningPerServitor(int restaurangId, DateTime startDatum, DateTime slutDatum)
        {
            var bestallningar = _unitOfWork.BestallningRepository.GetQuery()
                .Where(b => b.RestaurangID == restaurangId && b.Datum >= startDatum && b.Datum <= slutDatum)
                .Include(b => b.AnvandareBeh)
                .ToList();

            var servitorStatistik = bestallningar
                .GroupBy(b => new { b.AnvandarID, b.AnvandareBeh.Namn })
                .Select(g => new ServitorStatistik
                {
                    AnvandarID = g.Key.AnvandarID,
                    Namn = g.Key.Namn,
                    AntalTransaktioner = g.Count(),
                    TotalForsaljning = g.Sum(b => b.TotalSumma)
                })
                .OrderByDescending(s => s.TotalForsaljning)
                .ToList();

            return servitorStatistik;
        }

        /// <summary>
        /// Hämtar antal bokningar, bord och gäster för en restaurang
        /// </summary>
        public BokningsStatistik GetBokningsStatistik(int restaurangId, DateTime startDatum, DateTime slutDatum)
        {
            var bokningar = _unitOfWork.BokningRepository.GetQuery()
                .Where(b => b.RestaurangID == restaurangId && b.Datum >= startDatum && b.Datum <= slutDatum)
                .ToList();

            return new BokningsStatistik
            {
                AntalBokningar = bokningar.Count,
                AntalGaster = bokningar.Sum(b => b.AntalGaster),
                AntalUnikalaBord = bokningar.Select(b => b.BordID).Distinct().Count()
            };
        }

        #endregion

        #region VD Statistik

        /// <summary>
        /// Hämtar försäljningsstatistik för hela koncernen
        /// </summary>
        public ForsaljningsStatistik GetForsaljningKoncern(DateTime startDatum, DateTime slutDatum)
        {
            var bestallningar = _unitOfWork.BestallningRepository.GetQuery()
                .Where(b => b.Datum >= startDatum && b.Datum <= slutDatum)
                .Include(b => b.BestallningsRader)
                .ThenInclude(br => br.Meny)
                .ToList();

            // Beräkna mat och alkohol baserat på meny-kategori
            decimal matSumma = 0;
            decimal alkoholSumma = 0;

            foreach (var bestallning in bestallningar)
            {
                foreach (var rad in bestallning.BestallningsRader)
                {
                    if (IsAlkoholKategori(rad.Meny.Kategori))
                    {
                        alkoholSumma += rad.Summa;
                    }
                    else
                    {
                        matSumma += rad.Summa;
                    }
                }
            }

            return new ForsaljningsStatistik
            {
                TotalForsaljning = bestallningar.Sum(b => b.TotalSumma),
                MatSumma = matSumma,
                AlkoholSumma = alkoholSumma,
                AntalTransaktioner = bestallningar.Count
            };
        }

        /// <summary>
        /// Hämtar försäljning per region
        /// </summary>
        public List<RegionStatistik> GetForsaljningPerRegion(DateTime startDatum, DateTime slutDatum)
        {
            var bestallningar = _unitOfWork.BestallningRepository.GetQuery()
                .Where(b => b.Datum >= startDatum && b.Datum <= slutDatum)
                .Include(b => b.Restaurang)
                .ThenInclude(r => r.Region)
                .Include(b => b.BestallningsRader)
                .ThenInclude(br => br.Meny)
                .ToList();

            var regionStatistik = bestallningar
                .GroupBy(b => new { b.Restaurang.RegionID, b.Restaurang.Region.Regionnamn })
                .Select(g =>
                {
                    decimal matSumma = 0;
                    decimal alkoholSumma = 0;

                    foreach (var best in g)
                    {
                        foreach (var rad in best.BestallningsRader)
                        {
                            if (IsAlkoholKategori(rad.Meny.Kategori))
                                alkoholSumma += rad.Summa;
                            else
                                matSumma += rad.Summa;
                        }
                    }

                    return new RegionStatistik
                    {
                        RegionID = g.Key.RegionID,
                        RegionNamn = g.Key.Regionnamn,
                        TotalForsaljning = g.Sum(b => b.TotalSumma),
                        MatSumma = matSumma,
                        AlkoholSumma = alkoholSumma,
                        AntalTransaktioner = g.Count()
                    };
                })
                .OrderByDescending(r => r.TotalForsaljning)
                .ToList();

            return regionStatistik;
        }

        /// <summary>
        /// Hämtar mest sålda rätter för hela koncernen
        /// </summary>
        public List<RattStatistik> GetMestSaldaRatterKoncern(DateTime startDatum, DateTime slutDatum, int antal = 10)
        {
            var bestallningar = _unitOfWork.BestallningRepository.GetQuery()
                .Where(b => b.Datum >= startDatum && b.Datum <= slutDatum)
                .Include(b => b.BestallningsRader)
                .ThenInclude(br => br.Meny)
                .ToList();

            var rattStatistik = bestallningar
                .SelectMany(b => b.BestallningsRader)
                .GroupBy(br => new { br.MenyID, br.Meny.Rattnamn, br.Meny.Kategori })
                .Select(g => new RattStatistik
                {
                    MenyID = g.Key.MenyID,
                    Rattnamn = g.Key.Rattnamn,
                    Kategori = g.Key.Kategori,
                    AntalSalda = g.Sum(br => br.Antal),
                    TotalForsaljning = g.Sum(br => br.Summa)
                })
                .OrderByDescending(r => r.AntalSalda)
                .Take(antal)
                .ToList();

            return rattStatistik;
        }

        /// <summary>
        /// Hämtar minst sålda rätter för hela koncernen
        /// </summary>
        public List<RattStatistik> GetMinstSaldaRatterKoncern(DateTime startDatum, DateTime slutDatum, int antal = 10)
        {
            var bestallningar = _unitOfWork.BestallningRepository.GetQuery()
                .Where(b => b.Datum >= startDatum && b.Datum <= slutDatum)
                .Include(b => b.BestallningsRader)
                .ThenInclude(br => br.Meny)
                .ToList();

            var rattStatistik = bestallningar
                .SelectMany(b => b.BestallningsRader)
                .GroupBy(br => new { br.MenyID, br.Meny.Rattnamn, br.Meny.Kategori })
                .Select(g => new RattStatistik
                {
                    MenyID = g.Key.MenyID,
                    Rattnamn = g.Key.Rattnamn,
                    Kategori = g.Key.Kategori,
                    AntalSalda = g.Sum(br => br.Antal),
                    TotalForsaljning = g.Sum(br => br.Summa)
                })
                .OrderBy(r => r.AntalSalda)
                .Take(antal)
                .ToList();

            return rattStatistik;
        }

        /// <summary>
        /// Hämtar försäljning per restaurang för hela koncernen
        /// </summary>
        public List<RestaurangStatistik> GetForsaljningPerRestaurang(DateTime startDatum, DateTime slutDatum)
        {
            var bestallningar = _unitOfWork.BestallningRepository.GetQuery()
                .Where(b => b.Datum >= startDatum && b.Datum <= slutDatum)
                .Include(b => b.Restaurang)
                .Include(b => b.BestallningsRader)
                .ThenInclude(br => br.Meny)
                .ToList();

            var restaurangStatistik = bestallningar
                .GroupBy(b => new { b.RestaurangID, b.Restaurang.Restaurangnamn, b.Restaurang.RegionID })
                .Select(g =>
                {
                    decimal matSumma = 0;
                    decimal alkoholSumma = 0;

                    foreach (var best in g)
                    {
                        foreach (var rad in best.BestallningsRader)
                        {
                            if (IsAlkoholKategori(rad.Meny.Kategori))
                                alkoholSumma += rad.Summa;
                            else
                                matSumma += rad.Summa;
                        }
                    }

                    return new RestaurangStatistik
                    {
                        RestaurangID = g.Key.RestaurangID,
                        RestaurangNamn = g.Key.Restaurangnamn,
                        RegionID = g.Key.RegionID,
                        TotalForsaljning = g.Sum(b => b.TotalSumma),
                        MatSumma = matSumma,
                        AlkoholSumma = alkoholSumma,
                        AntalTransaktioner = g.Count()
                    };
                })
                .OrderByDescending(r => r.TotalForsaljning)
                .ToList();

            return restaurangStatistik;
        }

        #endregion
    }

    #region Statistik DTOs

    public class ForsaljningsStatistik
    {
        public decimal TotalForsaljning { get; set; }
        public decimal MatSumma { get; set; }
        public decimal AlkoholSumma { get; set; }
        public int AntalTransaktioner { get; set; }
    }

    public class RattStatistik
    {
        public int MenyID { get; set; }
        public string Rattnamn { get; set; }
        public string Kategori { get; set; }
        public int AntalSalda { get; set; }
        public decimal TotalForsaljning { get; set; }
    }

    public class ServitorStatistik
    {
        public int AnvandarID { get; set; }
        public string Namn { get; set; }
        public int AntalTransaktioner { get; set; }
        public decimal TotalForsaljning { get; set; }
    }

    public class BokningsStatistik
    {
        public int AntalBokningar { get; set; }
        public int AntalGaster { get; set; }
        public int AntalUnikalaBord { get; set; }
    }

    public class RegionStatistik
    {
        public int RegionID { get; set; }
        public string RegionNamn { get; set; }
        public decimal TotalForsaljning { get; set; }
        public decimal MatSumma { get; set; }
        public decimal AlkoholSumma { get; set; }
        public int AntalTransaktioner { get; set; }
    }

    public class RestaurangStatistik
    {
        public int RestaurangID { get; set; }
        public string RestaurangNamn { get; set; }
        public int RegionID { get; set; }
        public decimal TotalForsaljning { get; set; }
        public decimal MatSumma { get; set; }
        public decimal AlkoholSumma { get; set; }
        public int AntalTransaktioner { get; set; }
    }

    #endregion
}