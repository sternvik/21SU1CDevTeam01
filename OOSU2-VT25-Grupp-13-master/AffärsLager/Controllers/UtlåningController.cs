using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitetsLager;
using DataLager;

namespace AffärsLager
{
    public class UtlåningController
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork();


        // Registrera en ny utlåning
        public void RegistreraUtlåning(Utlåning utlåning)
        {
            // Hämtar medlem och utrustning baserat på ID
            var medlem = _unitOfWork.MedlemRepository.FirstOrDefault(p => p.MedlemID == utlåning.MedlemID);
            var utrustning = _unitOfWork.UtrustningRepository.FirstOrDefault(p => p.UtrustningID == utlåning.UtrustningID);

            // Kontrollera om medlemmet finns
            if (medlem == null)
            {
                throw new InvalidOperationException($"Fel: Medlem med ID {utlåning.MedlemID} hittades inte.");
            }

            // Kontrollera om utrustningen finns
            if (utrustning == null)
            {
                throw new InvalidOperationException($"Fel: Utrustning med inventarienummer {utlåning.UtrustningID} hittades inte.");
            }

            // Kontrollera om det finns tillgänglig utrustning
            if (utrustning.Tillgängliga <= 0)
            {
                throw new InvalidOperationException($"Fel: Utrustning med inventarienummer {utlåning.UtrustningID} är inte tillgänglig.");
            }

            // Kontrollera om utlåningsdatum är innan återlämningsdatum
            if (utlåning.Återlämningsdatum.HasValue && utlåning.UtLåningsdatum > utlåning.Återlämningsdatum.Value)
            {
                throw new InvalidOperationException("Fel: Utlåningsdatum kan inte vara senare än återlämningsdatum.");
            }


            // Lägger till utlåningen i databasen och uppdaterar utrustningens tillgänglighet
            _unitOfWork.UtlåningRepository.Add(utlåning);
            utrustning.Tillgängliga--;

            // Om återlämningsdatum finns öka tillgängligheten för utrustningen.
            if (utlåning.Återlämningsdatum.HasValue)
            {
                utrustning.Tillgängliga++;
            }

            _unitOfWork.Save(); // Spara ändringarna i databasen
        }


        // Registrera en återlämning av utrustning
        public void RegistreraÅterlämning(Utlåning utlåningattåterlämna)
        {
            // Hämtar utlåningen baserat på medlemID, utrustningID och att återlämningsdatum är null
            var utlåning = _unitOfWork.UtlåningRepository.FirstOrDefault(p =>
                    p.MedlemID == utlåningattåterlämna.MedlemID &&
                    p.UtrustningID == utlåningattåterlämna.UtrustningID &&
                    p.Återlämningsdatum == null);

            // Kontrollera om utlåningen finns
            if (utlåning == null)
            {
                throw new InvalidOperationException("Fel: Ingen aktiv utlåning hittades för denna utrustning.");
            }

            // Kontrollera om återlämningsdatum är före utlåningsdatum
            if (utlåningattåterlämna.Återlämningsdatum < utlåning.UtLåningsdatum)
            {
                throw new InvalidOperationException("Fel: Återlämningsdatum kan inte vara före utlåningsdatum.");
            }

            // Uppdatera återlämningsdatum i databasen
            utlåning.Återlämningsdatum = utlåningattåterlämna.Återlämningsdatum;

            // Hämtar utrustning och uppdaterar tillgängligheten
            var utrustning = _unitOfWork.UtrustningRepository.FirstOrDefault(p => p.UtrustningID == utlåningattåterlämna.UtrustningID);
            if (utrustning != null)
            {
                utrustning.Tillgängliga++;
                _unitOfWork.Save(); // Spara ändringarna i databasen
            }
        }

        // Hämtar alla aktiva utlåningar (utan återlämningsdatum eller med återlämningsdatum i framtiden)
        public IEnumerable<Utlåning> HämtaAllaAktivaUtlåningar()
        {
            return _unitOfWork.UtlåningRepository.GetAll().Where(u => u.Återlämningsdatum == null || u.Återlämningsdatum > DateTime.Now);
        }

        // Hämtar arkiverade utlåningar (med återlämningsdatum som är tidigare än nuvarande tid)
        public IEnumerable<Utlåning> HämtaArkiveradeUtlånignar()
        {
            return _unitOfWork.UtlåningRepository.GetAll().Where(u => u.Återlämningsdatum < DateTime.Now);
        }

        // Hämtar alla utlåningar i systemet.
        public IEnumerable<Utlåning> HämtaALlaUtlåningar()
        {
            return _unitOfWork.UtlåningRepository.GetAll();
        }
    }
}
