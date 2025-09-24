using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AffärsLager
{
    public class UtrustningController
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork();


        // Registrerar ny utrustning i systemet och sparar ändringarna i databasen.
        public void RegistreraUtrustning(Utrustning utrustning)
        {
            _unitOfWork.UtrustningRepository.Add(utrustning);
            _unitOfWork.Save();
        }

        // Uppdaterar informationen för en befintlig utrustning.
        public void UppdateraUtrustning(Utrustning utrustningattUppdatera)
        {
            // Hämtar utrustning från databasen baserat på utrustningens ID.
            var utrustning = _unitOfWork.UtrustningRepository.FirstOrDefault(m => m.UtrustningID == utrustningattUppdatera.UtrustningID);
            if (utrustning != null)
            {
                // Uppdaterar information för utrustningen.
                utrustning.Namn = utrustningattUppdatera.Namn;
                utrustning.Tillgängliga = utrustningattUppdatera.Tillgängliga;
                utrustning.Kategori = utrustningattUppdatera.Kategori;
                utrustning.Skick = utrustningattUppdatera.Skick;
                _unitOfWork.Save(); // Sparar ändringarna i databasen.
            }
        }

        // Tar bort en utrustning från systemet.
        public void TaBortUtrustning(Utrustning utrustningattTabort)
        {
            // Hämtar utrustning från databasen baserat på utrustningens ID.
            var utrustning = _unitOfWork.UtrustningRepository.FirstOrDefault(m => m.UtrustningID == utrustningattTabort.UtrustningID);

            if (utrustning != null)
            {
                _unitOfWork.UtrustningRepository.Remove(utrustning); // Tar bort utrustningen.
                _unitOfWork.Save(); // Sparar ändringarna i databasen.
            }
        }

        // Visar status för en utrustning.
        public bool VisaUtrustningStatus(Utrustning utrustningattVisa)
        {
            var utrustning = _unitOfWork.UtrustningRepository.FirstOrDefault(u => u.UtrustningID == utrustningattVisa.UtrustningID);
            return utrustning != null && utrustning.Tillgängliga > 0;
        }

        // Hämtar en lista med alla utrustningskategorier.
        public List<string> HämtaKategorier()
        {
            return new List<string> { "Racketar", "Bollar", "Klubbor", "Mål" };
        }

        // Hämtar en lista med alla möjliga skick för utrustning.
        public List<string> HämtaSkick()
        {
            return new List<string> { "Ny", "God", "Sliten", "Trasig" };
        }

        // Hämtar alla utrustningsobjekt från databasen.
        public IEnumerable<Utrustning> HämtaAllUtrustning()
        {
            return _unitOfWork.UtrustningRepository.GetAll();
        }

        // Hämtar alla utrustningsobjekt som är tillgängliga (dvs har mer än 0 tillgängliga enheter).
        public IEnumerable<Utrustning> HämtaTillgängligUtrustning()
        {
            return _unitOfWork.UtrustningRepository.GetAll().Where(u => u.Tillgängliga > 0);
        }

        // Hämtar alla utrustningsobjekt som är saknade (dvs har inga tillgängliga enheter).
        public IEnumerable<Utrustning> HämtaSaknadUtrustning()
        {
            return _unitOfWork.UtrustningRepository.GetAll().Where(u => u.Tillgängliga <= 0);
        }

        // Hämtar alla utrustningsobjekt som är trasiga.
        public IEnumerable<Utrustning> HämtaTrasigUtrustning()
        {
            return _unitOfWork.UtrustningRepository.GetAll().Where(u => u.Skick == "Trasig");
        }
    }
}
