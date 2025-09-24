using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLager;
using EntitetsLager;

namespace AffärsLager
{
    // TräningspassController hanterar logik för träningspass
    public class TräningspassController
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork();


        // Kontrollera om en tränare är tillgänglig vid ett visst datum och tid.
        public bool ÄrTränareTillgänglig(int tränareID, DateTime datum, TimeSpan tid)
        {
            // Hämta ett träningspass där tränaren är bokad vid samma datum och tid.
            var träningspass = _unitOfWork.TräningspassRepository.FirstOrDefault(t => t.TränareID == tränareID && t.Datum.Date == datum.Date && t.Tid == tid);

            // Om inget träningspass hittades är tränaren tillgänglig.
            return träningspass == null;
        }

        // Kontrollera om en plats är tillgänglig vid ett visst datum och tid.
        public bool ÄrPlatsTillgänglig(string plats, DateTime datum, TimeSpan tid)
        {
            // Hämta ett träningspass där samma plats är bokad vid samma datum och tid.
            var träningspass = _unitOfWork.TräningspassRepository.FirstOrDefault(t => t.Plats == plats && t.Datum.Date == datum.Date && t.Tid == tid);

            // Om inget träningspass hittades är platsen tillgänglig.
            return träningspass == null;
        }

        // Skapa ett nytt träningspass om tränaren och platsen är tillgängliga.
        public void SkapaTräningspass(Träningspass träningspass)
        {
            // Hämta tränaren för att säkerställa att den finns i databasen.
            var tränare = _unitOfWork.TränareRepository.FirstOrDefault(p => p.TränareID == träningspass.TränareID);

            // Kontrollera om tränaren är tillgänglig vid den angivna tiden.
            if (!ÄrTränareTillgänglig(träningspass.TränareID, träningspass.Datum, träningspass.Tid))
            {
                throw new InvalidOperationException("Tränaren är redan bokad vid denna tid.");
            }

            // Kontrollera om platsen är tillgänglig vid den angivna tiden.
            if (!ÄrPlatsTillgänglig(träningspass.Plats, träningspass.Datum, träningspass.Tid))
            {
                throw new InvalidOperationException("Platsen är redan bokad vid denna tid.");
            }

            // Lägg till det nya träningspasset och spara det i databasen.
            _unitOfWork.TräningspassRepository.Add(träningspass);
            _unitOfWork.Save();

        }

        // Ta bort ett träningspass från databasen.
        public void TaBortTräningspass(Träningspass träningspassattTabort)
        {
            // Hämta träningspasset som ska tas bort baserat på träningspassID.
            var träningspass = _unitOfWork.TräningspassRepository.FirstOrDefault(t => t.TräningspassID == träningspassattTabort.TräningspassID);
            if (träningspass != null)
            {
                // Ta bort träningspasset.
                _unitOfWork.TräningspassRepository.Remove(träningspass);
                _unitOfWork.Save(); // Spara förändringarna i databasen.

            }

        }

        // Redigera ett befintligt träningspass.
        public void RedigeraTräningspass(Träningspass träningspassattändra)
        {
            // Hämta träningspasset som ska redigeras baserat på träningspassID.
            var träningspass = _unitOfWork.TräningspassRepository.FirstOrDefault(t => t.TräningspassID == träningspassattändra.TräningspassID);
            if (träningspass != null) // Om träningspasset finns.
            {
                // Uppdatera träningspassets detaljer.
                träningspass.TränareID = träningspassattändra.TränareID;
                träningspass.Aktivitet = träningspassattändra.Aktivitet;
                träningspass.Datum = träningspassattändra.Datum;
                träningspass.Tid = träningspassattändra.Tid;
                träningspass.Plats = träningspassattändra.Plats;

                _unitOfWork.Save(); // Spara förändringarna i databasen.
            }
        }

        // Hämta en lista med tillgängliga lokaler baserat på aktivitet.
        public List<string> HämtaLokalerFörAktivitet(string aktivitet)
        {
            var lokaler = new List<string>(); // Skapa en lista för lokaler.

            // Beroende på aktivitet, lägg till specifika lokaler i listan.
            switch (aktivitet)
            {
                case "Paddel":
                    lokaler.Add("Paddelsal A");
                    lokaler.Add("Paddelsal B");
                    break;
                case "Tennis":
                    lokaler.Add("Tennisplan A");
                    lokaler.Add("Tennisplan B");
                    break;
                case "Pingis":
                    lokaler.Add("Pingisbord A");
                    lokaler.Add("Pingisbord B");
                    break;
                case "Squash":
                    lokaler.Add("Squashsal A");
                    lokaler.Add("Squashsal B");
                    break;
                case "Badminton":
                    lokaler.Add("Badmintonplan A");
                    lokaler.Add("Badmintonplan B");
                    break;
                case "Innebandy":
                    lokaler.Add("Innebandyplan A");
                    break;
            }

            return lokaler; // Returnera listan med lokaler.
        }

        // Hämta en lista med tider som är tillgängliga för aktiviteter.
        public List<string> HämtaTiderFörAktivitet()
        {
            List<string> tider = new List<string>(); // Skapa en lista för tider.

            // Lägg till tider mellan 08:00 och 21:00.
            for (int i = 8; i < 21; i++)
            {
                string tid = $"{i}:00";
                tider.Add(tid); // Lägg till varje timme.
            }

            return tider; // Returnera listan med tider.
        }

        public int HämtaMaxAntalDeltagare(string aktivitet)
        {
            int max = 0;

            switch (aktivitet)
            {
                case "Paddel":
                    max = 4;
                    break;
                case "Tennis":
                    max = 4;
                    break;
                case "Pingis":
                    max = 2;
                    break;
                case "Squash":
                    max = 4;
                    break;
                case "Badminton":
                    max = 4;
                    break;
                case "Innebandy":
                    max = 10;
                    break;
            }
   
            return max;
        }

        // Hämta alla träningspass från databasen.
        public IEnumerable<Träningspass> HämtaAllaTräningspass()
        {
            return _unitOfWork.TräningspassRepository.GetAll(); // Hämta och returnera alla träningspass.
        }

        // Hämta tränare för ett specifikt pass.
        public Tränare HämtaTränareFörTräningspass(int tränareID)
        {
            return _unitOfWork.TränareRepository.FirstOrDefault(t => t.TränareID == tränareID);
        }
    }
    
}
