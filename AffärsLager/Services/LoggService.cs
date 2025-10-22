using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AffärsLager.Services
{
    /// <summary>
    /// Service för att hantera loggning till både databas och textfil
    /// </summary>
    public class LoggService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly string _loggFilePath;

        public LoggService()
        {
            _unitOfWork = new UnitOfWork();

            // Skapa loggmapp om den inte finns
            var loggMapp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Loggar");
            if (!Directory.Exists(loggMapp))
            {
                Directory.CreateDirectory(loggMapp);
            }

            // Skapa loggfil med dagens datum
            var filnamn = $"SystemLogg_{DateTime.Now:yyyy-MM-dd}.txt";
            _loggFilePath = Path.Combine(loggMapp, filnamn);
        }

        /// <summary>
        /// Logga en händelse till både databas och textfil
        /// </summary>
        public void LoggaHandelse(int anvandarId, string modul, string handelse, string? detaljer = null)
        {
            try
            {
                // Spara till databas
                var systemlogg = new Systemlogg
                {
                    AnvandarID = anvandarId,
                    Modul = modul,
                    Handelse = handelse,
                    Datum = DateTime.Now.Date,
                    Tid = DateTime.Now.TimeOfDay,
                    IPAdress = "127.0.0.1"
                };

                _unitOfWork.SystemloggRepository.Add(systemlogg);
                _unitOfWork.Save();

                // Spara till textfil
                SkrivTillTextfil(anvandarId, modul, handelse, detaljer);
            }
            catch (Exception ex)
            {
                // Om loggning misslyckas, skriv bara till textfil som backup
                SkrivTillTextfil(anvandarId, modul, $"FEL VID LOGGNING: {handelse}", ex.Message);
            }
        }

        private void SkrivTillTextfil(int anvandarId, string modul, string handelse, string? detaljer)
        {
            try
            {
                var loggRad = new StringBuilder();
                loggRad.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
                loggRad.Append($"Personal-ID: {anvandarId} | ");
                loggRad.Append($"Modul: {modul} | ");
                loggRad.Append($"Händelse: {handelse}");

                if (!string.IsNullOrWhiteSpace(detaljer))
                {
                    loggRad.Append($" | Detaljer: {detaljer}");
                }

                loggRad.AppendLine();

                // Skriv till fil (thread-safe)
                lock (this)
                {
                    File.AppendAllText(_loggFilePath, loggRad.ToString(), Encoding.UTF8);
                }
            }
            catch
            {
                // Ignorera fel i filskrivning för att inte störa huvudapplikationen
            }
        }

        /// <summary>
        /// Hämta loggar från databas för en specifik period
        /// </summary>
        public List<Systemlogg> HamtaLoggar(DateTime? franDatum = null, DateTime? tillDatum = null,
            int? anvandarId = null, string? modul = null)
        {
            var query = _unitOfWork.SystemloggRepository.GetAll().AsQueryable();

            if (franDatum.HasValue)
                query = query.Where(l => l.Datum >= franDatum.Value.Date);

            if (tillDatum.HasValue)
                query = query.Where(l => l.Datum <= tillDatum.Value.Date);

            if (anvandarId.HasValue)
                query = query.Where(l => l.AnvandarID == anvandarId.Value);

            if (!string.IsNullOrWhiteSpace(modul))
                query = query.Where(l => l.Modul == modul);

            return query.OrderByDescending(l => l.Datum).ThenByDescending(l => l.Tid).ToList();
        }

        /// <summary>
        /// Hämta senaste loggarna (X antal)
        /// </summary>
        public List<Systemlogg> HamtaSenasteLoggar(int antal = 50, int? anvandarId = null)
        {
            var loggar = HamtaLoggar(anvandarId: anvandarId);
            return loggar.Take(antal).ToList();
        }

        /// <summary>
        /// Hämta loggar för en specifik modul
        /// </summary>
        public List<Systemlogg> HamtaLoggarForModul(string modul, DateTime? franDatum = null, DateTime? tillDatum = null)
        {
            return HamtaLoggar(franDatum, tillDatum, modul: modul);
        }

        /// <summary>
        /// Hämta loggar för en specifik användare
        /// </summary>
        public List<Systemlogg> HamtaLoggarForAnvandare(int anvandarId, DateTime? franDatum = null, DateTime? tillDatum = null)
        {
            return HamtaLoggar(franDatum, tillDatum, anvandarId: anvandarId);
        }

        /// <summary>
        /// Generera och öppna loggfil för hela koncernen
        /// </summary>
        public string GeneraLoggfilKoncern(DateTime? franDatum = null, DateTime? tillDatum = null)
        {
            var loggar = HamtaLoggar(franDatum, tillDatum);
            return GeneraLoggfil(loggar, "Koncern");
        }

        /// <summary>
        /// Generera och öppna loggfil för en specifik restaurang
        /// </summary>
        public string GeneraLoggfilRestaurang(int restaurangId, DateTime? franDatum = null, DateTime? tillDatum = null)
        {
            // Hämta alla användare som är kopplade till restaurangen
            var anvandare = _unitOfWork.AnvandareRepository.GetQuery()
                .Where(a => a.HemmarestaurangID == restaurangId)
                .Select(a => a.AnvandarID)
                .ToList();

            // Hämta loggar för dessa användare
            var loggar = _unitOfWork.SystemloggRepository.GetQuery()
                .Where(l => l.AnvandarID.HasValue && anvandare.Contains(l.AnvandarID.Value));

            if (franDatum.HasValue)
                loggar = loggar.Where(l => l.Datum >= franDatum.Value.Date);

            if (tillDatum.HasValue)
                loggar = loggar.Where(l => l.Datum <= tillDatum.Value.Date);

            var loggLista = loggar
                .OrderByDescending(l => l.Datum)
                .ThenByDescending(l => l.Tid)
                .ToList();

            // Hämta restaurangnamn
            var restaurang = _unitOfWork.RestaurangRepository.GetQuery().FirstOrDefault(r => r.RestaurangID == restaurangId);
            var restaurangNamn = restaurang?.Restaurangnamn ?? $"Restaurang_{restaurangId}";

            return GeneraLoggfil(loggLista, restaurangNamn);
        }

        private string GeneraLoggfil(List<Systemlogg> loggar, string beskrivning)
        {
            var exportMapp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoggExport");
            if (!Directory.Exists(exportMapp))
            {
                Directory.CreateDirectory(exportMapp);
            }

            var filnamn = $"Systemlogg_{beskrivning}_{DateTime.Now:yyyy-MM-dd_HHmmss}.txt";
            var filPath = Path.Combine(exportMapp, filnamn);

            using (var writer = new StreamWriter(filPath, false, Encoding.UTF8))
            {
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine($"                         SYSTEMLOGG - {beskrivning.ToUpper()}");
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine($"Genererad: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Antal händelser: {loggar.Count}");
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine();

                if (!loggar.Any())
                {
                    writer.WriteLine("Inga loggposter hittades för vald period.");
                }
                else
                {
                    foreach (var logg in loggar)
                    {
                        var anvandare = _unitOfWork.AnvandareRepository.GetQuery().FirstOrDefault(a => a.AnvandarID == (logg.AnvandarID ?? 0));
                        var anvandarNamn = anvandare?.Namn ?? "System";

                        writer.WriteLine($"[{logg.Datum:yyyy-MM-dd} {logg.Tid:hh\\:mm\\:ss}]");
                        writer.WriteLine($"  Personal: {anvandarNamn} (ID: {logg.AnvandarID ?? 0})");
                        writer.WriteLine($"  Modul: {logg.Modul}");
                        writer.WriteLine($"  Händelse: {logg.Handelse}");
                        if (!string.IsNullOrWhiteSpace(logg.IPAdress))
                        {
                            writer.WriteLine($"  IP-adress: {logg.IPAdress}");
                        }
                        writer.WriteLine();
                    }
                }

                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
                writer.WriteLine("                              SLUT PÅ RAPPORT");
                writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
            }

            return filPath;
        }
    }
}