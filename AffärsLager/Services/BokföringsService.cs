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
            decimal totalInklDricks = totalOmsattning + totalDricks;
            int antalTransaktioner = bestallningar.Count;
            decimal genomsnittKop = antalTransaktioner > 0 ? totalOmsattning / antalTransaktioner : 0;

            var filnamn = $"Bokföring_{restaurangNamn.Replace(" ", "_")}_{datum:yyyy-MM-dd}.txt";
            var filPath = Path.Combine(_exportMapp, filnamn);

            using (var writer = new StreamWriter(filPath, false, Encoding.UTF8))
            {
                writer.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
                writer.WriteLine("║                       DAGLIG BOKFÖRINGSUNDERLAG                               ║");
                writer.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════╝");
                writer.WriteLine();
                writer.WriteLine($"📅 Datum:           {datum:dddd, dd MMMM yyyy}", new System.Globalization.CultureInfo("sv-SE"));
                writer.WriteLine($"🏢 Restaurang:      {restaurangNamn}");
                writer.WriteLine($"📋 Restaurang-ID:   {restaurangId}");
                writer.WriteLine($"🕐 Genererad:       {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();
                writer.WriteLine("─────────────────────────────────────────────────────────────────────────────────");
                writer.WriteLine();
                writer.WriteLine("💰 FÖRSÄLJNINGSSAMMANSTÄLLNING");
                writer.WriteLine();
                writer.WriteLine($"   Mat (ex. alkohol):          {matSumma,15:N2} kr");
                writer.WriteLine($"   Alkoholhaltiga drycker:     {alkoholSumma,15:N2} kr");
                writer.WriteLine($"   ────────────────────────────────────────────");
                writer.WriteLine($"   Summa försäljning:          {totalOmsattning,15:N2} kr");
                writer.WriteLine($"   Dricks (ej moms):           {totalDricks,15:N2} kr");
                writer.WriteLine($"   ────────────────────────────────────────────");
                writer.WriteLine($"   TOTALT INKL. DRICKS:        {totalInklDricks,15:N2} kr");
                writer.WriteLine();
                writer.WriteLine("─────────────────────────────────────────────────────────────────────────────────");
                writer.WriteLine();
                writer.WriteLine("📊 TRANSAKTIONSSTATISTIK");
                writer.WriteLine();
                writer.WriteLine($"   Antal transaktioner:        {antalTransaktioner,15}");
                writer.WriteLine($"   Genomsnittligt köpvärde:    {genomsnittKop,15:N2} kr");
                writer.WriteLine($"   Mat/Alkohol-fördelning:     {(totalOmsattning > 0 ? (matSumma / totalOmsattning * 100) : 0),14:N1}% / {(totalOmsattning > 0 ? (alkoholSumma / totalOmsattning * 100) : 0):N1}%");
                writer.WriteLine();
                writer.WriteLine("─────────────────────────────────────────────────────────────────────────────────");
                writer.WriteLine();
                writer.WriteLine("💳 KASSASTATUS");
                writer.WriteLine();
                writer.WriteLine($"   Förväntat kassasaldo:       {totalOmsattning,15:N2} kr");
                writer.WriteLine($"   Dricks att fördela:         {totalDricks,15:N2} kr");
                writer.WriteLine();
                writer.WriteLine("─────────────────────────────────────────────────────────────────────────────────");
                writer.WriteLine();
                writer.WriteLine("📄 CSV-FORMAT FÖR IMPORT I BOKFÖRINGSSYSTEM:");
                writer.WriteLine();
                writer.WriteLine("Datum;Restaurang;RestaurangID;Mat;Alkohol;Summa;Dricks;Total;AntalTransaktioner");
                writer.WriteLine($"{datum:yyyy-MM-dd};{restaurangNamn};{restaurangId};{matSumma:F2};{alkoholSumma:F2};{totalOmsattning:F2};{totalDricks:F2};{totalInklDricks:F2};{antalTransaktioner}");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();
                writer.WriteLine("   📧 För frågor kontakta: ekonomi@restonation.se");
                writer.WriteLine("   ✅ Detta underlag är genererat automatiskt från RestoNation System");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
            }

            return filPath;
        }

        /// <summary>
        /// Generera bokföringsfil för alla restauranger för ett datum
        /// </summary>
        public string GeneraBokföringKoncern(DateTime datum)
        {
            var restauranger = _unitOfWork.RestaurangRepository.GetAll().OrderBy(r => r.Restaurangnamn).ToList();

            var filnamn = $"Bokföring_KONCERN_{datum:yyyy-MM-dd}.txt";
            var filPath = Path.Combine(_exportMapp, filnamn);

            decimal totalMatSumma = 0;
            decimal totalAlkoholSumma = 0;
            decimal totalDricks = 0;
            decimal totalOmsattning = 0;
            int totalAntalTransaktioner = 0;

            using (var writer = new StreamWriter(filPath, false, Encoding.UTF8))
            {
                writer.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
                writer.WriteLine("║                    KONCERNBOKFÖRING - RESTONATION                            ║");
                writer.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════╝");
                writer.WriteLine();
                writer.WriteLine($"📅 Datum:        {datum:dddd, dd MMMM yyyy}", new System.Globalization.CultureInfo("sv-SE"));
                writer.WriteLine($"🕐 Genererad:    {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"🏢 Restauranger: {restauranger.Count} st");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();

                writer.WriteLine("📋 RESTAURANGSPECIFIK UPPDELNING:");
                writer.WriteLine();

                foreach (var restaurang in restauranger)
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

                    writer.WriteLine($"   🏪 {restaurang.Restaurangnamn,-25}");
                    writer.WriteLine($"      Mat:             {matSumma,15:N2} kr");
                    writer.WriteLine($"      Alkohol:         {alkoholSumma,15:N2} kr");
                    writer.WriteLine($"      Summa:           {restaurangOmsattning,15:N2} kr");
                    writer.WriteLine($"      Dricks:          {dricks,15:N2} kr");
                    writer.WriteLine($"      Transaktioner:   {antalTransaktioner,15}");
                    writer.WriteLine();

                    totalMatSumma += matSumma;
                    totalAlkoholSumma += alkoholSumma;
                    totalDricks += dricks;
                    totalOmsattning += restaurangOmsattning;
                    totalAntalTransaktioner += antalTransaktioner;
                }

                decimal totalInklDricks = totalOmsattning + totalDricks;
                decimal genomsnittPerRestaurang = restauranger.Count > 0 ? totalOmsattning / restauranger.Count : 0;

                writer.WriteLine("─────────────────────────────────────────────────────────────────────────────────");
                writer.WriteLine();
                writer.WriteLine("💰 KONCERNSAMMANFATTNING:");
                writer.WriteLine();
                writer.WriteLine($"   Mat (ex. alkohol):          {totalMatSumma,15:N2} kr");
                writer.WriteLine($"   Alkoholhaltiga drycker:     {totalAlkoholSumma,15:N2} kr");
                writer.WriteLine($"   ────────────────────────────────────────────");
                writer.WriteLine($"   Summa försäljning:          {totalOmsattning,15:N2} kr");
                writer.WriteLine($"   Dricks (ej moms):           {totalDricks,15:N2} kr");
                writer.WriteLine($"   ────────────────────────────────────────────");
                writer.WriteLine($"   TOTALT INKL. DRICKS:        {totalInklDricks,15:N2} kr");
                writer.WriteLine();
                writer.WriteLine($"   Totalt transaktioner:       {totalAntalTransaktioner,15}");
                writer.WriteLine($"   Genomsnitt per restaurang:  {genomsnittPerRestaurang,15:N2} kr");
                writer.WriteLine($"   Mat/Alkohol-fördelning:     {(totalOmsattning > 0 ? (totalMatSumma / totalOmsattning * 100) : 0),14:N1}% / {(totalOmsattning > 0 ? (totalAlkoholSumma / totalOmsattning * 100) : 0):N1}%");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();
                writer.WriteLine("📄 CSV-FORMAT FÖR IMPORT I BOKFÖRINGSSYSTEM:");
                writer.WriteLine();
                writer.WriteLine("Datum;Enhet;Mat;Alkohol;Summa;Dricks;Total;AntalTransaktioner");

                foreach (var restaurang in restauranger)
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
                    decimal restaurangTotal = restaurangOmsattning + dricks;
                    writer.WriteLine($"{datum:yyyy-MM-dd};{restaurang.Restaurangnamn};{matSumma:F2};{alkoholSumma:F2};{restaurangOmsattning:F2};{dricks:F2};{restaurangTotal:F2};{bestallningar.Count}");
                }

                writer.WriteLine($"{datum:yyyy-MM-dd};KONCERN TOTALT;{totalMatSumma:F2};{totalAlkoholSumma:F2};{totalOmsattning:F2};{totalDricks:F2};{totalInklDricks:F2};{totalAntalTransaktioner}");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();
                writer.WriteLine("   📧 För frågor kontakta: ekonomi@restonation.se");
                writer.WriteLine("   ✅ Detta underlag är genererat automatiskt från RestoNation System");
                writer.WriteLine();
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
