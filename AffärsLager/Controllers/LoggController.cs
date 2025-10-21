using AffärsLager.Services;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class LoggController
    {
        private readonly LoggService _loggService;

        public LoggController()
        {
            _loggService = new LoggService();
        }

        public void LoggaHandelse(int anvandarId, string modul, string handelse, string? detaljer = null)
        {
            _loggService.LoggaHandelse(anvandarId, modul, handelse, detaljer);
        }

        public List<Systemlogg> HamtaLoggar(DateTime? franDatum = null, DateTime? tillDatum = null,
            int? anvandarId = null, string? modul = null)
        {
            return _loggService.HamtaLoggar(franDatum, tillDatum, anvandarId, modul);
        }

        public void LoggaInloggning(int anvandarId, string anvandarnamn, string roll)
        {
            _loggService.LoggaHandelse(anvandarId, "Login", "Användare loggade in",
                $"Användarnamn: {anvandarnamn}, Roll: {roll}");
        }

        public void LoggaUtloggning(int anvandarId, string anvandarnamn)
        {
            _loggService.LoggaHandelse(anvandarId, "Login", "Användare loggade ut",
                $"Användarnamn: {anvandarnamn}");
        }
    }
}