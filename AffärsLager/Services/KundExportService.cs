
using DataLager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AffärsLager.Services
{
    /// <summary>
    /// KundExportService - Exporterar kundlistor till CSV-format
    /// Används av marknadsavdelningen för att skicka nyhetsbrev och kampanjer
    /// Kan filtrera på: Region, Lojalitetsnivå, Restaurang, Datum
    /// Använder svensk CSV-standard (semikolon som separator, UTF-8 encoding)
    /// </summary>
    public class KundExportService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly string _exportMapp;

        public KundExportService()
        {
            _unitOfWork = new UnitOfWork();

            // Skapa exportmapp om den inte finns
            // CSV-filer sparas i en "KundExport"-mapp
            _exportMapp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KundExport");
            if (!Directory.Exists(_exportMapp))
            {
                Directory.CreateDirectory(_exportMapp);
            }
        }

        /// <summary>
        /// Exportera alla kunder till CSV
        /// </summary>
        public string ExporteraAllaKunder()
        {
            var kunder = _unitOfWork.KundRepository.GetAll().ToList();
            var filnamn = $"Kundlista_ALLA_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
            var filSokväg = Path.Combine(_exportMapp, filnamn);

            var csv = new StringBuilder();

            // Header med svensk CSV-standard (semikolon)
            csv.AppendLine("KundID;Namn;Email;Telefon;LojalitetsNiva;LojalitetsPoang;Region;Hemmarestaurang;Skapad");

            foreach (var kund in kunder)
            {
                var regionNamn = _unitOfWork.RegionRepository
                    .FirstOrDefault(r => r.RegionID == kund.RegionID)?.Regionnamn ?? "Okänd";

                var restaurangNamn = _unitOfWork.RestaurangRepository
                    .FirstOrDefault(r => r.RestaurangID == kund.HemmarestaurangID)?.Restaurangnamn ?? "Ingen";

                csv.AppendLine($"{kund.KundID};" +
                              $"{EscapeCsv(kund.Namn)};" +
                              $"{EscapeCsv(kund.Email ?? "")};" +
                              $"{EscapeCsv(kund.Telefon ?? "")};" +
                              $"{kund.LojalitetsNiva};" +
                              $"{kund.LojalitetsPoang};" +
                              $"{regionNamn};" +
                              $"{restaurangNamn};" +
                              $"{kund.SkapadDatum:yyyy-MM-dd}");
            }

            File.WriteAllText(filSokväg, csv.ToString(), Encoding.UTF8);
            return filSokväg;
        }

        /// <summary>
        /// Exportera kunder filtrerat per region
        /// </summary>
        public string ExporteraKunderPerRegion(int regionId)
        {
            var kunder = _unitOfWork.KundRepository.GetAll()
                .Where(k => k.RegionID == regionId)
                .ToList();

            var regionNamn = _unitOfWork.RegionRepository
                .FirstOrDefault(r => r.RegionID == regionId)?.Regionnamn ?? $"Region{regionId}";

            var filnamn = $"Kundlista_{regionNamn}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
            var filSokväg = Path.Combine(_exportMapp, filnamn);

            var csv = new StringBuilder();
            csv.AppendLine("KundID;Namn;Email;Telefon;LojalitetsNiva;LojalitetsPoang;Hemmarestaurang;Skapad");

            foreach (var kund in kunder)
            {
                var restaurangNamn = _unitOfWork.RestaurangRepository
                    .FirstOrDefault(r => r.RestaurangID == kund.HemmarestaurangID)?.Restaurangnamn ?? "Ingen";

                csv.AppendLine($"{kund.KundID};" +
                              $"{EscapeCsv(kund.Namn)};" +
                              $"{EscapeCsv(kund.Email ?? "")};" +
                              $"{EscapeCsv(kund.Telefon ?? "")};" +
                              $"{kund.LojalitetsNiva};" +
                              $"{kund.LojalitetsPoang};" +
                              $"{restaurangNamn};" +
                              $"{kund.SkapadDatum:yyyy-MM-dd}");
            }

            File.WriteAllText(filSokväg, csv.ToString(), Encoding.UTF8);
            return filSokväg;
        }

        /// <summary>
        /// Exportera kunder per lojalitetsnivå
        /// </summary>
        public string ExporteraKunderPerLojalitet(string lojalitetsNiva)
        {
            var kunder = _unitOfWork.KundRepository.GetAll()
                .Where(k => k.LojalitetsNiva == lojalitetsNiva)
                .ToList();

            var filnamn = $"Kundlista_{lojalitetsNiva}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
            var filSokväg = Path.Combine(_exportMapp, filnamn);

            var csv = new StringBuilder();
            csv.AppendLine("KundID;Namn;Email;Telefon;LojalitetsPoang;Region;Hemmarestaurang;Skapad");

            foreach (var kund in kunder)
            {
                var regionNamn = _unitOfWork.RegionRepository
                    .FirstOrDefault(r => r.RegionID == kund.RegionID)?.Regionnamn ?? "Okänd";

                var restaurangNamn = _unitOfWork.RestaurangRepository
                    .FirstOrDefault(r => r.RestaurangID == kund.HemmarestaurangID)?.Restaurangnamn ?? "Ingen";

                csv.AppendLine($"{kund.KundID};" +
                              $"{EscapeCsv(kund.Namn)};" +
                              $"{EscapeCsv(kund.Email ?? "")};" +
                              $"{EscapeCsv(kund.Telefon ?? "")};" +
                              $"{kund.LojalitetsPoang};" +
                              $"{regionNamn};" +
                              $"{restaurangNamn};" +
                              $"{kund.SkapadDatum:yyyy-MM-dd}");
            }

            File.WriteAllText(filSokväg, csv.ToString(), Encoding.UTF8);
            return filSokväg;
        }

        /// <summary>
        /// Exportera kunder med avancerad filtrering
        /// </summary>
        public string ExporteraKunderMedFilter(int? regionId = null, string? lojalitetsNiva = null,
            int? hemmarestaurangId = null, DateTime? skapadEfter = null)
        {
            var kunder = _unitOfWork.KundRepository.GetAll().AsQueryable();

            // Applicera filter
            if (regionId.HasValue)
                kunder = kunder.Where(k => k.RegionID == regionId.Value);

            if (!string.IsNullOrWhiteSpace(lojalitetsNiva))
                kunder = kunder.Where(k => k.LojalitetsNiva == lojalitetsNiva);

            if (hemmarestaurangId.HasValue)
                kunder = kunder.Where(k => k.HemmarestaurangID == hemmarestaurangId.Value);

            if (skapadEfter.HasValue)
                kunder = kunder.Where(k => k.SkapadDatum >= skapadEfter.Value);

            var kundLista = kunder.ToList();

            // Skapa filnamn baserat på filter
            var filterNamn = "Filtrerad";
            if (regionId.HasValue)
                filterNamn += $"_Region{regionId}";
            if (!string.IsNullOrWhiteSpace(lojalitetsNiva))
                filterNamn += $"_{lojalitetsNiva}";

            var filnamn = $"Kundlista_{filterNamn}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
            var filSokväg = Path.Combine(_exportMapp, filnamn);

            var csv = new StringBuilder();
            csv.AppendLine("KundID;Namn;Email;Telefon;LojalitetsNiva;LojalitetsPoang;Region;Hemmarestaurang;Skapad");

            foreach (var kund in kundLista)
            {
                var regionNamn = _unitOfWork.RegionRepository
                    .FirstOrDefault(r => r.RegionID == kund.RegionID)?.Regionnamn ?? "Okänd";

                var restaurangNamn = _unitOfWork.RestaurangRepository
                    .FirstOrDefault(r => r.RestaurangID == kund.HemmarestaurangID)?.Restaurangnamn ?? "Ingen";

                csv.AppendLine($"{kund.KundID};" +
                              $"{EscapeCsv(kund.Namn)};" +
                              $"{EscapeCsv(kund.Email ?? "")};" +
                              $"{EscapeCsv(kund.Telefon ?? "")};" +
                              $"{kund.LojalitetsNiva};" +
                              $"{kund.LojalitetsPoang};" +
                              $"{regionNamn};" +
                              $"{restaurangNamn};" +
                              $"{kund.SkapadDatum:yyyy-MM-dd}");
            }

            File.WriteAllText(filSokväg, csv.ToString(), Encoding.UTF8);
            return filSokväg;
        }

        /// <summary>
        /// Exportera endast email-adresser för nyhetsbrev
        /// </summary>
        public string ExporteraEmailLista(int? regionId = null, string? lojalitetsNiva = null)
        {
            var kunder = _unitOfWork.KundRepository.GetAll()
                .Where(k => !string.IsNullOrWhiteSpace(k.Email))
                .AsQueryable();

            if (regionId.HasValue)
                kunder = kunder.Where(k => k.RegionID == regionId.Value);

            if (!string.IsNullOrWhiteSpace(lojalitetsNiva))
                kunder = kunder.Where(k => k.LojalitetsNiva == lojalitetsNiva);

            var kundLista = kunder.ToList();

            var filterNamn = "ALLA";
            if (regionId.HasValue || !string.IsNullOrWhiteSpace(lojalitetsNiva))
            {
                filterNamn = "Filtrerad";
                if (regionId.HasValue)
                    filterNamn += $"_Region{regionId}";
                if (!string.IsNullOrWhiteSpace(lojalitetsNiva))
                    filterNamn += $"_{lojalitetsNiva}";
            }

            var filnamn = $"EmailLista_{filterNamn}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
            var filSokväg = Path.Combine(_exportMapp, filnamn);

            var csv = new StringBuilder();
            csv.AppendLine("Email;Namn;LojalitetsNiva");

            foreach (var kund in kundLista)
            {
                csv.AppendLine($"{EscapeCsv(kund.Email)};" +
                              $"{EscapeCsv(kund.Namn)};" +
                              $"{kund.LojalitetsNiva}");
            }

            File.WriteAllText(filSokväg, csv.ToString(), Encoding.UTF8);
            return filSokväg;
        }

        /// <summary>
        /// Escape special characters för CSV
        /// </summary>
        private string EscapeCsv(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // Om texten innehåller semikolon, citattecken eller radbrytning, wrappa i quotes
            if (text.Contains(';') || text.Contains('"') || text.Contains('\n'))
            {
                return $"\"{text.Replace("\"", "\"\"")}\"";
            }

            return text;
        }

        /// <summary>
        /// Exportera kunder för en specifik restaurang (för restaurangchefer)
        /// </summary>
        public string ExporteraKunderPerRestaurang(int restaurangId)
        {
            var kunder = _unitOfWork.KundRepository.GetAll()
                .Where(k => k.HemmarestaurangID == restaurangId)
                .ToList();

            var restaurangNamn = _unitOfWork.RestaurangRepository
                .FirstOrDefault(r => r.RestaurangID == restaurangId)?.Restaurangnamn ?? $"Restaurang{restaurangId}";

            var filnamn = $"Kundlista_{restaurangNamn}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
            var filSokväg = Path.Combine(_exportMapp, filnamn);

            var csv = new StringBuilder();
            csv.AppendLine("KundID;Namn;Email;Telefon;LojalitetsNiva;LojalitetsPoang;Region;Skapad;AntalBesok");

            foreach (var kund in kunder)
            {
                var regionNamn = _unitOfWork.RegionRepository
                    .FirstOrDefault(r => r.RegionID == kund.RegionID)?.Regionnamn ?? "Okänd";

                // Räkna antal bokningar för kunden på denna restaurang
                var antalBesok = _unitOfWork.BokningRepository.GetAll()
                    .Count(b => b.KundID == kund.KundID &&
                               b.RestaurangID == restaurangId &&
                               b.Status != "Avbokad");

                csv.AppendLine($"{kund.KundID};" +
                              $"{EscapeCsv(kund.Namn)};" +
                              $"{EscapeCsv(kund.Email ?? "")};" +
                              $"{EscapeCsv(kund.Telefon ?? "")};" +
                              $"{kund.LojalitetsNiva};" +
                              $"{kund.LojalitetsPoang};" +
                              $"{regionNamn};" +
                              $"{kund.SkapadDatum:yyyy-MM-dd};" +
                              $"{antalBesok}");
            }

            File.WriteAllText(filSokväg, csv.ToString(), Encoding.UTF8);
            return filSokväg;
        }

        /// <summary>
        /// Hämta statistik om kundbasen
        /// </summary>
        public Dictionary<string, int> HamtaKundStatistik()
        {
            var kunder = _unitOfWork.KundRepository.GetAll().ToList();

            return new Dictionary<string, int>
            {
                { "TotalAntalKunder", kunder.Count },
                { "Brons", kunder.Count(k => k.LojalitetsNiva == "Brons") },
                { "Silver", kunder.Count(k => k.LojalitetsNiva == "Silver") },
                { "Guld", kunder.Count(k => k.LojalitetsNiva == "Guld") },
                { "MedEmail", kunder.Count(k => !string.IsNullOrWhiteSpace(k.Email)) },
                { "MedTelefon", kunder.Count(k => !string.IsNullOrWhiteSpace(k.Telefon)) }
            };
        }
    }
}
