using EntitetsLager;
using DataLager;
using System.Collections.Concurrent;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// AnvandareController - Hanterar användare, inloggning och sessioner
    /// Ansvarar för autentisering, rollhantering och sessionstimeouts
    /// </summary>
    public class AnvandareController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        // Håller koll på inloggade användare och när de senast var aktiva
        private static readonly ConcurrentDictionary<int, DateTime> _aktivaSessioner = new();

        // Session timeout: 30 minuter inaktivitet innan automatisk utloggning
        private static readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Autentiserar en användare med användarnamn och lösenord
        /// Startar en session om inloggning lyckas
        /// </summary>
        /// <param name="anvandarnamn">Användarnamn</param>
        /// <param name="losenord">Lösenord (plaintext i demo)</param>
        /// <returns>True om inloggning lyckades, annars false</returns>
        public bool AutentiseraAnvandare(string anvandarnamn, string losenord)
        {
            if (string.IsNullOrWhiteSpace(anvandarnamn) || string.IsNullOrWhiteSpace(losenord))
                return false;

            var anvandare = _unitOfWork.AnvandareRepository.FirstOrDefault(a =>
                a.Anvandarnamn == anvandarnamn &&
                a.Losenord == losenord &&
                a.Aktiv);

            if (anvandare != null)
            {
                StaraSession(anvandare.AnvandarID);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Hämtar en inloggad användare baserat på användarnamn
        /// Kontrollerar att användaren har en aktiv session
        /// </summary>
        /// <param name="anvandarnamn">Användarnamn</param>
        /// <returns>Användaren om inloggad, annars null</returns>
        public Anvandare? HamtaInloggadAnvandare(string anvandarnamn)
        {
            if (string.IsNullOrWhiteSpace(anvandarnamn))
                return null;

            var anvandare = _unitOfWork.AnvandareRepository.FirstOrDefault(a =>
                a.Anvandarnamn == anvandarnamn &&
                a.Aktiv);

            if (anvandare != null && ÄrAnvandareInloggad(anvandare.AnvandarID))
            {
                return anvandare;
            }

            return null;
        }

        /// <summary>
        /// Hämtar användare baserat på användar-ID
        /// Returnerar endast aktiva användare
        /// </summary>
        /// <param name="anvandarID">Användarens ID</param>
        /// <returns>Användaren om aktiv, annars null</returns>
        public Anvandare? HamtaAnvandareById(int anvandarID)
        {
            return _unitOfWork.AnvandareRepository.FirstOrDefault(a =>
                a.AnvandarID == anvandarID &&
                a.Aktiv);
        }

        /// <summary>
        /// Kontrollerar om en användare är inloggad
        /// Kollar session timeout (30 minuter) och loggar ut automatiskt om tid har gått ut
        /// </summary>
        /// <param name="anvandarID">Användarens ID</param>
        /// <returns>True om användaren har aktiv session, annars false</returns>
        public bool ÄrAnvandareInloggad(int anvandarID)
        {
            if (_aktivaSessioner.TryGetValue(anvandarID, out DateTime senastAktiv))
            {
                // Kolla om mindre än 30 minuter har gått sedan senast aktiv
                if (DateTime.Now - senastAktiv <= _sessionTimeout)
                {
                    UppdateraSessionAktivitet(anvandarID);
                    return true;
                }
                else
                {
                    // Session har gått ut, logga ut automatiskt
                    LoggaUtAnvandare(anvandarID);
                }
            }
            return false;
        }

        /// <summary>
        /// Loggar ut en användare genom att ta bort deras session
        /// </summary>
        /// <param name="anvandarID">Användarens ID</param>
        public void LoggaUtAnvandare(int anvandarID)
        {
            _aktivaSessioner.TryRemove(anvandarID, out _);
        }

        /// <summary>
        /// Kontrollerar om en användare har en specifik roll
        /// Används för att kontrollera behörighet
        /// </summary>
        /// <param name="anvandarID">Användarens ID</param>
        /// <param name="roll">Roll att kolla (VD, Restaurangchef, Personal, Admin)</param>
        /// <returns>True om användaren har rollen</returns>
        public bool HarRoll(int anvandarID, string roll)
        {
            var anvandare = HamtaAnvandareById(anvandarID);
            return anvandare?.Roll == roll;
        }

        /// <summary>
        /// Kontrollerar om en användare har någon av flera roller
        /// Användbart för funktioner som flera roller ska ha tillgång till
        /// </summary>
        /// <param name="anvandarID">Användarens ID</param>
        /// <param name="roller">Array med roller (t.ex. "VD", "Restaurangchef")</param>
        /// <returns>True om användaren har någon av rollerna</returns>
        public bool HarRoll(int anvandarID, params string[] roller)
        {
            var anvandare = HamtaAnvandareById(anvandarID);
            return anvandare != null && roller.Contains(anvandare.Roll);
        }

        /// <summary>
        /// Hämtar alla tillgängliga roller i systemet
        /// Används för dropdown-menyer vid skapande av användare
        /// </summary>
        /// <returns>Lista med alla roller</returns>
        public List<string> HamtaAllaTillgangligaRoller()
        {
            return new List<string> { "Servitör", "Admin", "Restaurangchef", "VD" };
        }

        /// <summary>
        /// Hämtar hemmarestaurang för en användare
        /// Personal har oftast en hemmarestaurang de jobbar på
        /// </summary>
        /// <param name="anvandarID">Användarens ID</param>
        /// <returns>Restaurang-ID om användaren har hemmarestaurang, annars null</returns>
        public int? HamtaHemmarestaurang(int anvandarID)
        {
            var anvandare = HamtaAnvandareById(anvandarID);
            return anvandare?.HemmarestaurangID;
        }

        /// <summary>
        /// Hämtar alla aktiva användare i systemet
        /// </summary>
        /// <returns>Lista med alla aktiva användare</returns>
        public List<Anvandare> HamtaAllaAktiviraAnvandare()
        {
            return _unitOfWork.AnvandareRepository.Find(a => a.Aktiv).ToList();
        }

        /// <summary>
        /// Hämtar alla användare med en specifik roll
        /// T.ex. alla Servitörer eller alla Restaurangchefer
        /// </summary>
        /// <param name="roll">Roll att filtrera på</param>
        /// <returns>Lista med användare som har rollen</returns>
        public List<Anvandare> HamtaAnvandareByRoll(string roll)
        {
            return _unitOfWork.AnvandareRepository.Find(a => a.Roll == roll && a.Aktiv).ToList();
        }

        /// <summary>
        /// Hämtar användare baserat på ID (inklusive inaktiva)
        /// Används vid admin-hantering
        /// </summary>
        /// <param name="anvandarId">Användarens ID</param>
        /// <returns>Användaren om den finns, annars null</returns>
        public Anvandare? HamtaAnvandareMedId(int anvandarId)
        {
            try
            {
                return _unitOfWork.AnvandareRepository.FirstOrDefault(a => a.AnvandarID == anvandarId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av användare med ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Söker efter användare baserat på namn, användarnamn eller roll
        /// Används i admin-gränssnitt för att hitta användare
        /// </summary>
        /// <param name="namn">Namn att söka efter (valfritt)</param>
        /// <param name="anvandarnamn">Användarnamn att söka efter (valfritt)</param>
        /// <param name="roll">Roll att filtrera på (valfritt)</param>
        /// <returns>Lista med matchande användare</returns>
        public List<Anvandare> SokAnvandare(string? namn = null, string? anvandarnamn = null, string? roll = null)
        {
            var query = _unitOfWork.AnvandareRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(namn))
                query = query.Where(a => a.Namn != null && a.Namn.ToLower().Contains(namn.Trim().ToLower()));

            if (!string.IsNullOrWhiteSpace(anvandarnamn))
                query = query.Where(a => a.Anvandarnamn != null && a.Anvandarnamn.ToLower().Contains(anvandarnamn.Trim().ToLower()));

            if (!string.IsNullOrWhiteSpace(roll))
                query = query.Where(a => a.Roll != null && a.Roll.ToLower().Contains(roll.Trim().ToLower()));

            return query.OrderBy(a => a.Namn).ToList();
        }

        /// <summary>
        /// Skapar en ny användare i systemet
        /// Validerar att användarnamn inte redan finns
        /// </summary>
        /// <param name="anvandare">Användarobjekt med alla uppgifter</param>
        /// <returns>True om användaren skapades, annars false</returns>
        public bool SkapaAnvandare(Anvandare anvandare)
        {
            // Validera att alla obligatoriska fält finns
            if (anvandare == null || string.IsNullOrWhiteSpace(anvandare.Anvandarnamn) ||
                string.IsNullOrWhiteSpace(anvandare.Losenord) || string.IsNullOrWhiteSpace(anvandare.Namn) ||
                string.IsNullOrWhiteSpace(anvandare.Roll))
                return false;

            // Kolla att användarnamnet inte redan finns
            var befintlig = _unitOfWork.AnvandareRepository.FirstOrDefault(a => a.Anvandarnamn == anvandare.Anvandarnamn);
            if (befintlig != null)
                return false;

            _unitOfWork.AnvandareRepository.Add(anvandare);
            _unitOfWork.Save();

            // Verifiera att användaren sparades korrekt
            var sparad = _unitOfWork.AnvandareRepository.FirstOrDefault(a => a.Anvandarnamn == anvandare.Anvandarnamn);
            return sparad != null;
        }

        /// <summary>
        /// Uppdaterar en befintlig användares uppgifter
        /// Kan ändra namn, användarnamn, lösenord, roll, aktiv-status och hemmarestaurang
        /// </summary>
        /// <param name="anvandare">Användarobjekt med nya uppgifter</param>
        /// <returns>True om uppdateringen lyckades</returns>
        public bool UppdateraAnvandare(Anvandare anvandare)
        {
            if (anvandare == null)
                return false;

            var befintlig = HamtaAnvandareMedId(anvandare.AnvandarID);
            if (befintlig == null)
                return false;

            // Uppdatera alla fält
            befintlig.Namn = anvandare.Namn;
            befintlig.Anvandarnamn = anvandare.Anvandarnamn;
            befintlig.Losenord = anvandare.Losenord;
            befintlig.Roll = anvandare.Roll;
            befintlig.Aktiv = anvandare.Aktiv;
            befintlig.HemmarestaurangID = anvandare.HemmarestaurangID;

            _unitOfWork.Save();
            return true;
        }

        /// <summary>
        /// Tar bort en användare från systemet
        /// OBS: Detta tar bort användaren permanent
        /// </summary>
        /// <param name="anvandarId">ID för användaren som ska tas bort</param>
        /// <returns>True om borttagningen lyckades</returns>
        public bool TaBortAnvandare(int anvandarId)
        {
            var anv = HamtaAnvandareMedId(anvandarId);
            if (anv == null)
                return false;

            _unitOfWork.AnvandareRepository.Remove(anv);
            _unitOfWork.Save();
            return true;
        }

        /// <summary>
        /// Ändrar lösenord för en användare
        /// Används när användare vill byta sitt lösenord
        /// </summary>
        /// <param name="anvandarId">Användarens ID</param>
        /// <param name="nyttLosenord">Nytt lösenord</param>
        /// <returns>True om lösenordet ändrades</returns>
        public bool AndraLosenord(int anvandarId, string nyttLosenord)
        {
            if (string.IsNullOrWhiteSpace(nyttLosenord))
                return false;

            var anvandare = HamtaAnvandareMedId(anvandarId);
            if (anvandare == null)
                return false;

            anvandare.Losenord = nyttLosenord.Trim();
            _unitOfWork.Save();
            return true;
        }

        /// <summary>
        /// Startar en ny session för en användare
        /// Sparar aktuell tid som senaste aktivitet
        /// </summary>
        /// <param name="anvandarID">Användarens ID</param>
        private void StaraSession(int anvandarID)
        {
            _aktivaSessioner.AddOrUpdate(anvandarID, DateTime.Now, (key, oldValue) => DateTime.Now);
        }

        /// <summary>
        /// Uppdaterar senaste aktivitetstid för en användare
        /// Anropas varje gång användaren gör något i systemet
        /// </summary>
        /// <param name="anvandarID">Användarens ID</param>
        private void UppdateraSessionAktivitet(int anvandarID)
        {
            if (_aktivaSessioner.ContainsKey(anvandarID))
            {
                _aktivaSessioner[anvandarID] = DateTime.Now;
            }
        }

        /// <summary>
        /// Rensar alla inaktiva sessioner (över 30 minuter gamla)
        /// Kan anropas periodiskt för att städa upp gamla sessioner
        /// </summary>
        public void RensaInaktivaVSessioner()
        {
            var inaktivaAnvandare = _aktivaSessioner
                .Where(kvp => DateTime.Now - kvp.Value > _sessionTimeout)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var anvandarID in inaktivaAnvandare)
            {
                LoggaUtAnvandare(anvandarID);
            }
        }
    }
}
