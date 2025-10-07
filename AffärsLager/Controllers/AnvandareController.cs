using EntitetsLager;
using DataLager;
using System.Collections.Concurrent;

namespace AffärsLager.Controllers
{
    public class AnvandareController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();
        private static readonly ConcurrentDictionary<int, DateTime> _aktivaSessioner = new();
        private static readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(30);

        

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

        public Anvandare? HamtaAnvandareById(int anvandarID)
        {
            return _unitOfWork.AnvandareRepository.FirstOrDefault(a =>
                a.AnvandarID == anvandarID &&
                a.Aktiv);
        }

        public bool ÄrAnvandareInloggad(int anvandarID)
        {
            if (_aktivaSessioner.TryGetValue(anvandarID, out DateTime senastAktiv))
            {
                if (DateTime.Now - senastAktiv <= _sessionTimeout)
                {
                    UppdateraSessionAktivitet(anvandarID);
                    return true;
                }
                else
                {
                    LoggaUtAnvandare(anvandarID);
                }
            }
            return false;
        }

        public void LoggaUtAnvandare(int anvandarID)
        {
            _aktivaSessioner.TryRemove(anvandarID, out _);
        }

        public bool HarRoll(int anvandarID, string roll)
        {
            var anvandare = HamtaAnvandareById(anvandarID);
            return anvandare?.Roll == roll;
        }

        public bool HarRoll(int anvandarID, params string[] roller)
        {
            var anvandare = HamtaAnvandareById(anvandarID);
            return anvandare != null && roller.Contains(anvandare.Roll);
        }

        public List<string> HamtaAllaTillgangligaRoller()
        {
            return new List<string> { "Servitör", "Admin", "Restaurangchef", "VD" };
        }

        public int? HamtaHemmarestaurang(int anvandarID)
        {
            var anvandare = HamtaAnvandareById(anvandarID);
            return anvandare?.HemmarestaurangID;
        }

        public List<Anvandare> HamtaAllaAktiviraAnvandare()
        {
            return _unitOfWork.AnvandareRepository.Find(a => a.Aktiv).ToList();
        }

        public List<Anvandare> HamtaAnvandareByRoll(string roll)
        {
            return _unitOfWork.AnvandareRepository.Find(a => a.Roll == roll && a.Aktiv).ToList();
        }

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

        public bool SkapaAnvandare(Anvandare anvandare)
        {
            if (anvandare == null || string.IsNullOrWhiteSpace(anvandare.Anvandarnamn) ||
                string.IsNullOrWhiteSpace(anvandare.Losenord) || string.IsNullOrWhiteSpace(anvandare.Namn) ||
                string.IsNullOrWhiteSpace(anvandare.Roll))
                return false;

            var befintlig = _unitOfWork.AnvandareRepository.FirstOrDefault(a => a.Anvandarnamn == anvandare.Anvandarnamn);
            if (befintlig != null)
                return false;

            _unitOfWork.AnvandareRepository.Add(anvandare);
            _unitOfWork.Save();

            var sparad = _unitOfWork.AnvandareRepository.FirstOrDefault(a => a.Anvandarnamn == anvandare.Anvandarnamn);
            return sparad != null;
        }

        public bool UppdateraAnvandare(Anvandare anvandare)
        {
            if (anvandare == null)
                return false;

            var befintlig = HamtaAnvandareMedId(anvandare.AnvandarID);
            if (befintlig == null)
                return false;

            befintlig.Namn = anvandare.Namn;
            befintlig.Anvandarnamn = anvandare.Anvandarnamn;
            befintlig.Losenord = anvandare.Losenord;
            befintlig.Roll = anvandare.Roll;
            befintlig.Aktiv = anvandare.Aktiv;
            befintlig.HemmarestaurangID = anvandare.HemmarestaurangID;

            _unitOfWork.Save();
            return true;
        }

        public bool TaBortAnvandare(int anvandarId)
        {
            var anv = HamtaAnvandareMedId(anvandarId);
            if (anv == null)
                return false;

            _unitOfWork.AnvandareRepository.Remove(anv);
            _unitOfWork.Save();
            return true;
        }

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

        private void StaraSession(int anvandarID)
        {
            _aktivaSessioner.AddOrUpdate(anvandarID, DateTime.Now, (key, oldValue) => DateTime.Now);
        }

        private void UppdateraSessionAktivitet(int anvandarID)
        {
            if (_aktivaSessioner.ContainsKey(anvandarID))
            {
                _aktivaSessioner[anvandarID] = DateTime.Now;
            }
        }

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