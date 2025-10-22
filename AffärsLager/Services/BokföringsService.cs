using DataLager;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AffärsLager.Services
{
    /// <summary>
    /// Service för att generera dagliga bokföringsunderlag för ekonomiavdelningen
    /// </summary>
    public class BokföringsService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly string _exportMapp;

        public BokföringsService()
        {
            _unitOfWork = new UnitOfWork();

            // Skapa exportmapp om den inte finns
            _exportMapp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bokföring");
            if (!Directory.Exists(_exportMapp))
            {
                Directory.CreateDirectory(_exportMapp);
            }
        }

        /// <summary>
        /// Generera bokföringsfil för en specifik restaurang och datum
        /// </summary>
        public string GeneraBokföringRestaurang(int restaurangId, DateTime datum)
        {
            var restaurang = _unitOfWork.RestaurangRepository.GetQuery()
                .FirstOrDefault(r => r.RestaurangID == restaurangId);
            var restaurangNamn = restaurang?.Restaurangnamn ?? $"Restaurang_{restaurangId}";

            // Hämta alla beställningar för dagen
            var bestallningar = _unitOfWork.BestallningRepository.GetAll()
                .Where(b => b.RestaurangID == restaurangId &&
                           b.Datum.Date == datum.Date)
                .ToList();

            decimal matSumma = 0;
            decimal alkoholSumma = 0;
            decimal totalDricks = 0;

            foreach (var bestallning in bestallningar)
            {
                totalDricks += bestallning.Dricks;

                var rader = _unitOfWork.BestallningsRadRepository.GetAll()
                    .Where(br => br.BestallningsID == bestallning.BestallningsID)
                    .ToList();

                foreach (var rad in rader)
                {
                    var meny = _unitOfWork.MenyRepository.GetQuery()
                        .FirstOrDefault(m => m.MenyID == rad.MenyID);

                    if (meny != null)
                    {
                        if (IsAlkoholKategori(meny.Kategori, meny.Rattnamn))
                        {
                            alkoholSumma += rad.Summa;
                        }
                        else
                        {
                            matSumma += rad.Summa;
                        }
                    }
                }
            }

            decimal totalOmsattning = matSumma + alkoholSumma;
            int antalTransaktioner = bestallningar.Count;

            var filnamn = $"Bokföring_{restaurangNamn}_{datum:yyyy-MM-dd}.csv";
            var filPath = Path.Combine(_exportMapp, filnamn);

            using (var writer = new StreamWriter(filPath, false, Encoding.UTF8))
            {
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine($"                    DAGLIG BOKFÖRING - {restaurangNamn.ToUpper()}");
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine($"Datum: {datum:yyyy-MM-dd}");
                writer.WriteLine($"Restaurang: {restaurangNamn}");
                writer.WriteLine($"Genererad: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();
                writer.WriteLine("FÖRSÄLJNING:");
                writer.WriteLine($"  Mat:                     {matSumma,12:N2} kr");
                writer.WriteLine($"  Alkohol:                 {alkoholSumma,12:N2} kr");
                writer.WriteLine($"  Dricks:                  {totalDricks,12:N2} kr");
                writer.WriteLine($"  ─────────────────────────────────────");
                writer.WriteLine($"  TOTAL OMSÄTTNING:        {totalOmsattning,12:N2} kr");
                writer.WriteLine();
                writer.WriteLine("TRANSAKTIONER:");
                writer.WriteLine($"  Antal transaktioner:     {antalTransaktioner,12}");
                writer.WriteLine($"  Genomsnittligt köp:      {(antalTransaktioner > 0 ? totalOmsattning / antalTransaktioner : 0),12:N2} kr");
                writer.WriteLine();
                writer.WriteLine("KASSASTATUS:");
                writer.WriteLine($"  Kassa (förväntat):       {totalOmsattning,12:N2} kr");
                writer.WriteLine($"  Kassa (räknat):          {"TBD",12}    (Kassafunktion ej implementerad)");
                writer.WriteLine($"  Avvikelse:               {"TBD",12}");
                writer.WriteLine();
                writer.WriteLine("KOMMENTAR:");
                writer.WriteLine("  Kassaavstämning implementeras i framtida version.");
                writer.WriteLine("  Betalmetoder (Kontant/Kort/Swish) implementeras i framtida version.");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();
                writer.WriteLine("CSV-FORMAT FÖR BOKFÖRINGSSYSTEM:");
                writer.WriteLine("Datum;Restaurang;Mat;Alkohol;Dricks;TotalOmsättning;AntalTransaktioner;KassaFörväntat;Status");
                writer.WriteLine($"{datum:yyyy-MM-dd};{restaurangNamn};{matSumma:F2};{alkoholSumma:F2};{totalDricks:F2};{totalOmsattning:F2};{antalTransaktioner};{totalOmsattning:F2};OK");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine("                      SKICKA TILL: ekonomi@restonation.se");
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
            }

            return filPath;
        }

        /// <summary>
        /// Generera bokföringsfil för alla restauranger för ett datum
        /// </summary>
        public string GeneraBokföringKoncern(DateTime datum)
        {
            var restauranger = _unitOfWork.RestaurangRepository.GetAll().ToList();

            var filnamn = $"Bokföring_Koncern_{datum:yyyy-MM-dd}.csv";
            var filPath = Path.Combine(_exportMapp, filnamn);

            decimal totalMatSumma = 0;
            decimal totalAlkoholSumma = 0;
            decimal totalDricks = 0;
            decimal totalOmsattning = 0;
            int totalAntalTransaktioner = 0;

            using (var writer = new StreamWriter(filPath, false, Encoding.UTF8))
            {
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine("              DAGLIG BOKFÖRING - HELA KONCERNEN");
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine($"Datum: {datum:yyyy-MM-dd}");
                writer.WriteLine($"Genererad: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();

                foreach (var restaurang in restauranger.OrderBy(r => r.Restaurangnamn))
                {
                    var bestallningar = _unitOfWork.BestallningRepository.GetAll()
                        .Where(b => b.RestaurangID == restaurang.RestaurangID &&
                                   b.Datum.Date == datum.Date)
                        .ToList();

                    decimal matSumma = 0;
                    decimal alkoholSumma = 0;
                    decimal dricks = 0;

                    foreach (var bestallning in bestallningar)
                    {
                        dricks += bestallning.Dricks;

                        var rader = _unitOfWork.BestallningsRadRepository.GetAll()
                            .Where(br => br.BestallningsID == bestallning.BestallningsID)
                            .ToList();

                        foreach (var rad in rader)
                        {
                            var meny = _unitOfWork.MenyRepository.GetQuery()
                                .FirstOrDefault(m => m.MenyID == rad.MenyID);

                            if (meny != null)
                            {
                                if (IsAlkoholKategori(meny.Kategori, meny.Rattnamn))
                                {
                                    alkoholSumma += rad.Summa;
                                }
                                else
                                {
                                    matSumma += rad.Summa;
                                }
                            }
                        }
                    }

                    decimal restaurangOmsattning = matSumma + alkoholSumma;
                    int antalTransaktioner = bestallningar.Count;

                    writer.WriteLine($"[ {restaurang.Restaurangnamn} ]");
                    writer.WriteLine($"  Mat:           {matSumma,12:N2} kr");
                    writer.WriteLine($"  Alkohol:       {alkoholSumma,12:N2} kr");
                    writer.WriteLine($"  Dricks:        {dricks,12:N2} kr");
                    writer.WriteLine($"  Omsättning:    {restaurangOmsattning,12:N2} kr");
                    writer.WriteLine($"  Transaktioner: {antalTransaktioner,12}");
                    writer.WriteLine();

                    totalMatSumma += matSumma;
                    totalAlkoholSumma += alkoholSumma;
                    totalDricks += dricks;
                    totalOmsattning += restaurangOmsattning;
                    totalAntalTransaktioner += antalTransaktioner;
                }

                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine("KONCERNSAMMANFATTNING:");
                writer.WriteLine($"  Mat:                     {totalMatSumma,12:N2} kr");
                writer.WriteLine($"  Alkohol:                 {totalAlkoholSumma,12:N2} kr");
                writer.WriteLine($"  Dricks:                  {totalDricks,12:N2} kr");
                writer.WriteLine($"  ─────────────────────────────────────");
                writer.WriteLine($"  TOTAL OMSÄTTNING:        {totalOmsattning,12:N2} kr");
                writer.WriteLine($"  Totalt transaktioner:    {totalAntalTransaktioner,12}");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();
                writer.WriteLine("CSV-FORMAT FÖR BOKFÖRINGSSYSTEM:");
                writer.WriteLine("Datum;Enhet;Mat;Alkohol;Dricks;TotalOmsättning;AntalTransaktioner;Status");

                foreach (var restaurang in restauranger.OrderBy(r => r.Restaurangnamn))
                {
                    var bestallningar = _unitOfWork.BestallningRepository.GetAll()
                        .Where(b => b.RestaurangID == restaurang.RestaurangID &&
                                   b.Datum.Date == datum.Date)
                        .ToList();

                    decimal matSumma = 0;
                    decimal alkoholSumma = 0;
                    decimal dricks = 0;

                    foreach (var bestallning in bestallningar)
                    {
                        dricks += bestallning.Dricks;
                        var rader = _unitOfWork.BestallningsRadRepository.GetAll()
                            .Where(br => br.BestallningsID == bestallning.BestallningsID)
                            .ToList();

                        foreach (var rad in rader)
                        {
                            var meny = _unitOfWork.MenyRepository.GetQuery()
                                .FirstOrDefault(m => m.MenyID == rad.MenyID);

                            if (meny != null)
                            {
                                if (IsAlkoholKategori(meny.Kategori, meny.Rattnamn))
                                    alkoholSumma += rad.Summa;
                                else
                                    matSumma += rad.Summa;
                            }
                        }
                    }

                    decimal restaurangOmsattning = matSumma + alkoholSumma;
                    writer.WriteLine($"{datum:yyyy-MM-dd};{restaurang.Restaurangnamn};{matSumma:F2};{alkoholSumma:F2};{dricks:F2};{restaurangOmsattning:F2};{bestallningar.Count};OK");
                }

                writer.WriteLine($"{datum:yyyy-MM-dd};KONCERN TOTALT;{totalMatSumma:F2};{totalAlkoholSumma:F2};{totalDricks:F2};{totalOmsattning:F2};{totalAntalTransaktioner};OK");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine("                      SKICKA TILL: ekonomi@restonation.se");
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
            }

            return filPath;
        }

        /// <summary>
        /// Hjälpmetod för att avgöra om en rätt är alkoholhaltig (kopierad från StatistikController)
        /// </summary>
        private bool IsAlkoholKategori(string kategori, string rattnamn = "")
        {
            if (string.IsNullOrEmpty(kategori)) return false;

            var lowerKategori = kategori.ToLower();
            var lowerRattnamn = rattnamn?.ToLower() ?? "";

            if (lowerKategori.Contains("alkoholfri") || lowerRattnamn.Contains("alkoholfri")) return false;

            if (lowerKategori == "dryck")
            {
                if (lowerRattnamn.Contains("läsk") ||
                    lowerRattnamn.Contains("kaffe") ||
                    lowerRattnamn.Contains("te") ||
                    lowerRattnamn.Contains("vatten") ||
                    lowerRattnamn.Contains("juice") ||
                    lowerRattnamn.Contains("smoothie"))
                {
                    return false;
                }

                if (lowerRattnamn.Contains("vin") ||
                    lowerRattnamn.Contains("öl") ||
                    lowerRattnamn.Contains("sprit") ||
                    lowerRattnamn.Contains("whisky") ||
                    lowerRattnamn.Contains("vodka") ||
                    lowerRattnamn.Contains("gin") ||
                    lowerRattnamn.Contains("rom") ||
                    lowerRattnamn.Contains("cognac") ||
                    lowerRattnamn.Contains("likör") ||
                    lowerRattnamn.Contains("cider") ||
                    lowerRattnamn.Contains("champagne") ||
                    lowerRattnamn.Contains("prosecco"))
                {
                    return true;
                }

                return true;
            }

            return lowerKategori.Contains("alkoholhaltig") ||
                   lowerKategori.Contains("öl") ||
                   lowerKategori.Contains("vin") ||
                   lowerKategori.Contains("sprit") ||
                   (lowerKategori.Contains("alkohol") && !lowerKategori.Contains("fri"));
        }
    }
}
