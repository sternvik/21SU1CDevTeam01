using AffärsLager.Services;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// LoggController - Hanterar systemloggning av användaraktiviteter
    /// Loggar viktiga händelser som inloggningar, ändringar i systemet etc.
    /// Används för säkerhet, felsökning och att följa upp vad som händer i systemet
    /// </summary>
    public class LoggController
    {
        private readonly LoggService _loggService;

        public LoggController()
        {
            _loggService = new LoggService();
        }

        /// <summary>
        /// Loggar en händelse i systemet
        /// T.ex. "Användare ändrade meny", "Bokning skapad", etc.
        /// </summary>
        /// <param name="anvandarId">Användarens ID som utförde händelsen</param>
        /// <param name="modul">Vilken del av systemet (t.ex. "Meny", "Bokning", "Login")</param>
        /// <param name="handelse">Beskrivning av vad som hände</param>
        /// <param name="detaljer">Extra information (valfritt)</param>
        public void LoggaHandelse(int anvandarId, string modul, string handelse, string? detaljer = null)
        {
            _loggService.LoggaHandelse(anvandarId, modul, handelse, detaljer);
        }

        /// <summary>
        /// Hämtar loggar med filtrering
        /// Kan filtrera på datum, användare och modul
        /// </summary>
        /// <param name="franDatum">Startdatum (valfritt)</param>
        /// <param name="tillDatum">Slutdatum (valfritt)</param>
        /// <param name="anvandarId">Filtrera på specifik användare (valfritt)</param>
        /// <param name="modul">Filtrera på specifik modul (valfritt)</param>
        /// <returns>Lista med matchande systemloggar</returns>
        public List<Systemlogg> HamtaLoggar(DateTime? franDatum = null, DateTime? tillDatum = null,
            int? anvandarId = null, string? modul = null)
        {
            return _loggService.HamtaLoggar(franDatum, tillDatum, anvandarId, modul);
        }

        /// <summary>
        /// Loggar när en användare loggar in i systemet
        /// Viktig säkerhetsfunktion för att spåra vem som använder systemet
        /// </summary>
        /// <param name="anvandarId">Användarens ID</param>
        /// <param name="anvandarnamn">Användarnamn</param>
        /// <param name="roll">Användarens roll (VD, Restaurangchef, etc.)</param>
        public void LoggaInloggning(int anvandarId, string anvandarnamn, string roll)
        {
            _loggService.LoggaHandelse(anvandarId, "Login", "Användare loggade in",
                $"Användarnamn: {anvandarnamn}, Roll: {roll}");
        }

        /// <summary>
        /// Loggar när en användare loggar ut från systemet
        /// </summary>
        /// <param name="anvandarId">Användarens ID</param>
        /// <param name="anvandarnamn">Användarnamn</param>
        public void LoggaUtloggning(int anvandarId, string anvandarnamn)
        {
            _loggService.LoggaHandelse(anvandarId, "Login", "Användare loggade ut",
                $"Användarnamn: {anvandarnamn}");
        }
    }
}