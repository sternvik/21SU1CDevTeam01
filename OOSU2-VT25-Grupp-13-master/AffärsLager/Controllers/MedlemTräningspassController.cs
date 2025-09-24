using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AffärsLager
{
    public class MedlemTräningspassController
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork(); //Hanterar transaktioner och repositories.


        // Metod för att lägga till en medlem till ett träningspass.
        public void LäggTillMedlemPåTräningspass(Medlem medlem, Träningspass träningspass)
        {
            if (medlem != null && träningspass != null)
            {
                // Kontrollera om medlemmen redan är anmäld till träningspasset
                var medlemTräningspass = _unitOfWork.MedlemTräningspassRepository
                    .FirstOrDefault(mt => mt.MedlemID == medlem.MedlemID && mt.TräningspassID == träningspass.TräningspassID);

                if (medlemTräningspass != null)
                {
                    throw new InvalidOperationException("Medlemmen är redan anmäld till detta träningspass.");
                }

                string nystatus;
                if (träningspass.Datum > DateTime.Now)
                {
                    nystatus = "Genomfört";
                }
                else
                {
                    nystatus = "Kommande";
                }

                // Skapa en ny koppling mellan medlem och träningspass
                var nyttMedlemTräningspass = new MedlemTräningspass
                {
                    MedlemID = medlem.MedlemID,
                    TräningspassID = träningspass.TräningspassID,
                    Status = nystatus
                };

                // Lägga till relationen i databasen och spara ändringar
                _unitOfWork.MedlemTräningspassRepository.Add(nyttMedlemTräningspass);
                _unitOfWork.Save();
            }
        }

        // Metod för att ta bort en medlem från ett träningspass.
        public void TaBortMedlemFrånPass(Medlem medlem, Träningspass träningspass)
        {
            // Hitta relationen mellan medlem och träningspass.
            var medlemTräningspass = _unitOfWork.MedlemTräningspassRepository
                .FirstOrDefault(mt => mt.MedlemID == medlem.MedlemID && mt.TräningspassID == träningspass.TräningspassID);

            if (medlemTräningspass != null)
            {
                //Tar bort relationen om den finns.
                _unitOfWork.MedlemTräningspassRepository.Remove(medlemTräningspass);
                _unitOfWork.Save();
            }
            else
            {
                throw new InvalidOperationException("Medlemmen är inte anmäld till detta träningspass.");
            }
        }

        // Metod för att hämta alla deltagare för ett specifikt träningspass
        public IEnumerable<Medlem> HämtaDeltagareFörTräningspass(int träningspassID)
        {
            //Hämta alla medlemmar som är anmälda till det specifika träningspasset.
            var deltagare = _unitOfWork.MedlemRepository.GetAll();
            var medlemTräningspassList = _unitOfWork.MedlemTräningspassRepository
                .GetAll()
                .Where(mt => mt.TräningspassID == träningspassID)
                .Select(mt => mt.MedlemID)
                .ToList();

            return deltagare.Where(m => medlemTräningspassList.Contains(m.MedlemID));
        }

        // Metod för att hämta alla tillgängliga medlemmar för ett specifikt träningspass.
        public IEnumerable<Medlem> HämtaTillgängligaMedlemmarFörTräningspass(int träningspassID)
        {
            // Hämta alla medlemmar som inte är anmälda till det specifika träningspasset.
            var allaMedlemmar = _unitOfWork.MedlemRepository.GetAll();
            var deltagareFörPasset = _unitOfWork.MedlemTräningspassRepository
                .GetAll()
                .Where(mt => mt.TräningspassID == träningspassID)
                .Select(mt => mt.MedlemID)
                .ToList();

            return allaMedlemmar.Where(m => !deltagareFörPasset.Contains(m.MedlemID));
        }

        public IEnumerable<Träningspass> HämtaTräningspassFörMedlem(int medlemID)
        {
            var träningspassList = _unitOfWork.TräningspassRepository.GetAll();
            var medlemTräningspassList = _unitOfWork.MedlemTräningspassRepository
                .GetAll()
                .Where(mt => mt.MedlemID == medlemID)
                .Select(mt => mt.TräningspassID)
                .ToList();

            return träningspassList.Where(tp => medlemTräningspassList.Contains(tp.TräningspassID));
        }
    }
}
