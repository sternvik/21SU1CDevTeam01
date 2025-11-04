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
        /// Skapar en TXT-fil med försäljningsdata uppdelat på Mat/Alkohol för ekonomiavdelningen
        /// </summary>
        /// <param name="restaurangId">ID för restaurangen som ska rapporteras</param>
        /// <param name="datum">Vilket datum bokföringen gäller för</param>
        /// <returns>Sökväg till den skapade filen</returns>
        public string GeneraBokföringRestaurang(int restaurangId, DateTime datum)
        {
            // Hämta restaurangnamn från databasen
            var restaurang = _unitOfWork.RestaurangRepository.GetQuery()
                .FirstOrDefault(r => r.RestaurangID == restaurangId);
            var restaurangNamn = restaurang?.Restaurangnamn ?? $"Restaurang_{restaurangId}";

            // Hämta alla beställningar för den valda dagen
            // Vi filtrerar på restaurang-ID och datum
            var bestallningar = _unitOfWork.BestallningRepository.GetAll()
                .Where(b => b.RestaurangID == restaurangId &&
                           b.Datum.Date == datum.Date)
                .ToList();

            // Initiera variabler för att samla ihop försäljningen
            decimal matSumma = 0;           // Summa för mat och alkoholfria drycker
            decimal alkoholSumma = 0;       // Summa för alkoholhaltiga drycker (viktigt för moms)
            decimal totalDricks = 0;        // Total dricks (moms räknas inte på dricks)

            // Gå igenom varje beställning och summera försäljningen
            foreach (var bestallning in bestallningar)
            {
                // Samla ihop dricks från alla beställningar
                totalDricks += bestallning.Dricks;

                // Hämta alla beställningsrader (enskilda maträtter) för denna beställning
                var rader = _unitOfWork.BestallningsRadRepository.GetAll()
                    .Where(br => br.BestallningsID == bestallning.BestallningsID)
                    .ToList();

                // Gå igenom varje rad och klassificera som mat eller alkohol
                foreach (var rad in rader)
                {
                    // Hämta menyobjektet för att se vilken kategori det tillhör
                    var meny = _unitOfWork.MenyRepository.GetQuery()
                        .FirstOrDefault(m => m.MenyID == rad.MenyID);

                    if (meny != null)
                    {
                        // Kontrollera om rätten är alkoholhaltig (olika moms gäller)
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

            // Beräkna sammanställning och statistik
            decimal totalOmsattning = matSumma + alkoholSumma;  // Total försäljning (exkl. dricks)
            decimal totalInklDricks = totalOmsattning + totalDricks;  // Total inkl. dricks
            int antalTransaktioner = bestallningar.Count;  // Antal beställningar under dagen
            decimal genomsnittKop = antalTransaktioner > 0 ? totalOmsattning / antalTransaktioner : 0;  // Genomsnittlig beställning

            // Skapa filnamn: Bokföring_RestaurangNamn_2025-01-04.txt
            var filnamn = $"Bokföring_{restaurangNamn.Replace(" ", "_")}_{datum:yyyy-MM-dd}.txt";
            var filPath = Path.Combine(_exportMapp, filnamn);

            // Skapa och skriv till fil med UTF-8 encoding för svenska tecken
            using (var writer = new StreamWriter(filPath, false, Encoding.UTF8))
            {
                // Skriv rubrik och header-information
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

                // Sektion 1: Försäljningssammanställning (viktigast för ekonomi)
                writer.WriteLine("💰 FÖRSÄLJNINGSSAMMANSTÄLLNING");
                writer.WriteLine();
                writer.WriteLine($"   Mat (ex. alkohol):          {matSumma,15:N2} kr");
                writer.WriteLine($"   Alkoholhaltiga drycker:     {alkoholSumma,15:N2} kr");
                writer.WriteLine($"   ────────────────────────────────────────────");
                writer.WriteLine($"   Summa försäljning:          {totalOmsattning,15:N2} kr");
                writer.WriteLine($"   Dricks (ej moms):           {totalDricks,15:N2} kr");  // Dricks är momsbefriad
                writer.WriteLine($"   ────────────────────────────────────────────");
                writer.WriteLine($"   TOTALT INKL. DRICKS:        {totalInklDricks,15:N2} kr");
                writer.WriteLine();
                writer.WriteLine("─────────────────────────────────────────────────────────────────────────────────");
                writer.WriteLine();

                // Sektion 2: Transaktionsstatistik (nyckeltal för analys)
                writer.WriteLine("📊 TRANSAKTIONSSTATISTIK");
                writer.WriteLine();
                writer.WriteLine($"   Antal transaktioner:        {antalTransaktioner,15}");
                writer.WriteLine($"   Genomsnittligt köpvärde:    {genomsnittKop,15:N2} kr");
                writer.WriteLine($"   Mat/Alkohol-fördelning:     {(totalOmsattning > 0 ? (matSumma / totalOmsattning * 100) : 0),14:N1}% / {(totalOmsattning > 0 ? (alkoholSumma / totalOmsattning * 100) : 0):N1}%");
                writer.WriteLine();
                writer.WriteLine("─────────────────────────────────────────────────────────────────────────────────");
                writer.WriteLine();

                // Sektion 3: Kassastatus
                writer.WriteLine("💳 KASSASTATUS");
                writer.WriteLine();
                writer.WriteLine($"   Förväntat kassasaldo:       {totalOmsattning,15:N2} kr");
                writer.WriteLine($"   Dricks att fördela:         {totalDricks,15:N2} kr");
                writer.WriteLine();
                writer.WriteLine("─────────────────────────────────────────────────────────────────────────────────");
                writer.WriteLine();

                // Sektion 4: CSV-format för import i bokföringssystem (Fortnox, Visma, etc.)
                writer.WriteLine("📄 CSV-FORMAT FÖR IMPORT I BOKFÖRINGSSYSTEM:");
                writer.WriteLine();
                writer.WriteLine("Datum;Restaurang;RestaurangID;Mat;Alkohol;Summa;Dricks;Total;AntalTransaktioner");
                writer.WriteLine($"{datum:yyyy-MM-dd};{restaurangNamn};{restaurangId};{matSumma:F2};{alkoholSumma:F2};{totalOmsattning:F2};{totalDricks:F2};{totalInklDricks:F2};{antalTransaktioner}");
                writer.WriteLine();

                // Footer
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();
                writer.WriteLine("   📧 För frågor kontakta: ekonomi@restonation.se");
                writer.WriteLine("   ✅ Detta underlag är genererat automatiskt från RestoNation System");
                writer.WriteLine();
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
            }

            return filPath;  // Returnera sökväg så användaren kan öppna filen
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
        /// Hjälpmetod för att avgöra om en rätt är alkoholhaltig
        /// Detta är viktigt för momsbokföringen eftersom alkohol och mat har olika momssatser
        /// </summary>
        /// <param name="kategori">Kategori från menyn (t.ex. "Dryck", "Mat")</param>
        /// <param name="rattnamn">Rättens namn (används för att detektera alkohol i namnet)</param>
        /// <returns>True om rätten är alkoholhaltig, annars false</returns>
        private bool IsAlkoholKategori(string kategori, string rattnamn = "")
        {
            // Om kategori saknas, anta att det inte är alkohol
            if (string.IsNullOrEmpty(kategori)) return false;

            // Konvertera till lowercase för säker jämförelse
            var lowerKategori = kategori.ToLower();
            var lowerRattnamn = rattnamn?.ToLower() ?? "";

            // Om det specifikt står "alkoholfri" så är det INTE alkohol
            if (lowerKategori.Contains("alkoholfri") || lowerRattnamn.Contains("alkoholfri")) return false;

            // Om kategorin är "Dryck" behöver vi kolla rättnamnet för att avgöra om det är alkohol
            if (lowerKategori == "dryck")
            {
                // Lista över alkoholfria drycker (kaffe, läsk, juice, etc.)
                if (lowerRattnamn.Contains("läsk") ||
                    lowerRattnamn.Contains("kaffe") ||
                    lowerRattnamn.Contains("te") ||
                    lowerRattnamn.Contains("vatten") ||
                    lowerRattnamn.Contains("juice") ||
                    lowerRattnamn.Contains("smoothie"))
                {
                    return false;  // Det är INTE alkohol
                }

                // Lista över alkoholhaltiga drycker (vin, öl, sprit, etc.)
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
                    return true;  // Det ÄR alkohol
                }

                return true;  // Om osäker, anta alkohol (säkrare för moms)
            }

            // Kolla om kategorin innehåller ord som indikerar alkohol
            return lowerKategori.Contains("alkoholhaltig") ||
                   lowerKategori.Contains("öl") ||
                   lowerKategori.Contains("vin") ||
                   lowerKategori.Contains("sprit") ||
                   (lowerKategori.Contains("alkohol") && !lowerKategori.Contains("fri"));
        }
    }
}
