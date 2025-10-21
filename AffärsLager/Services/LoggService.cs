using DataLager;
using EntitetsLager;
using System;
using System.IO;
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
                    Detaljer = detaljer,
                    Tidsstampel = DateTime.Now
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
                query = query.Where(l => l.Tidsstampel >= franDatum.Value);

            if (tillDatum.HasValue)
                query = query.Where(l => l.Tidsstampel <= tillDatum.Value);

            if (anvandarId.HasValue)
                query = query.Where(l => l.AnvandarID == anvandarId.Value);

            if (!string.IsNullOrWhiteSpace(modul))
                query = query.Where(l => l.Modul == modul);

            return query.OrderByDescending(l => l.Tidsstampel).ToList();
        }
    }
}
