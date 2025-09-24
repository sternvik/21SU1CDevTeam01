using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitetsLager;
using DataLager;
using System.Net.Http.Headers;

namespace AffärsLager
{
    // TränareController hanterar logiken för tränarhantering
    public class TränareController
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork();//Hanterar transaktioner och repositories.


        // Lägg till en ny tränare och sparar i databasen.
        public void LäggTillTränare(Tränare tränare)
        {
            _unitOfWork.TränareRepository.Add(tränare);
            _unitOfWork.Save();
        }

        // Uppdatera en tränare baserat på den tränare som skickas in.
        public void UppdateraTränare(Tränare tränareattUppdatera)
        {
            // Hämta den tränare som ska uppdateras baserat på tränareID.
            var tränare = _unitOfWork.TränareRepository.FirstOrDefault(m => m.TränareID == tränareattUppdatera.TränareID);
            if (tränare != null)
            {
                // Uppdatera tränarens uppgifter.
                tränare.Namn = tränareattUppdatera.Namn;
                tränare.Specialisering = tränareattUppdatera.Specialisering;
                tränare.Lösenord = tränareattUppdatera.Lösenord;

                _unitOfWork.Save(); // Spara ändringar i databasen.
            }
        }

        // Ta bort en tränare från databasen.
        public void TaBortTränare(Tränare tränareattTabort)
        {
            // Hämta den tränare som ska tas bort baserat på tränareID.
            var tränare = _unitOfWork.TränareRepository.FirstOrDefault(m => m.TränareID == tränareattTabort.TränareID);

            if (tränare != null) // Om tränaren finns i databasen.
            {
                _unitOfWork.TränareRepository.Remove(tränare); //Tar bort tränaren.
                _unitOfWork.Save(); // Spara ändringar i databasen.
            }
        }

        // Visa detaljer för en specifik tränare baserat på tränareID.
        public Tränare Visatränaredetaljer(int tränareID)
        {
            var tränare = _unitOfWork.TränareRepository.FirstOrDefault(m => m.TränareID == tränareID);
            return tränare; // Returnera tränaren om den finns annars returnera null.
        }

        // Hämta en lista av specialiseringar som tränare kan ha.
        public List<string> HämtaSpecialisering()
        {
            return new List<string> { "Paddel", "Tennis", "Pingis", "Squash", "Badminton", "Innebandy" };
        }

        // Hämta alla tränare från databasen.
        public IEnumerable<Tränare> HämtaAllaTränare()
        {
            return _unitOfWork.TränareRepository.GetAll(); // Returnera alla tränare från repositoriet.

        }
    }
}
