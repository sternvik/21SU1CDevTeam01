using DataLager.DataLager.Datalager;
using EntitetsLager.Entiteter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AffärsLager.Controllers
{
    public class TräningspassController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();


        public string LäggTillTräningspass(Träningspass träningspassAttLäggaTill)
        {

            träningspassAttLäggaTill.DeltagarAntal = 0;


            _unitOfWork.TräningspassRepository.Add(träningspassAttLäggaTill);
            _unitOfWork.Save();

            return "Medlem har lagts till!";
        }
        
        public Träningspass HämtaTräningspass(int träningspassID)
        {
            var träningspass = _unitOfWork.TräningspassRepository.FirstOrDefault(tp => tp.TräningspassID == träningspassID);
            if (träningspass == null)
            {
                throw new Exception("Träningspass ej hittat!");
            }
            return träningspass;
        }

        public void RemoveTräningspass(Träningspass träningspassattTabort)
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

        public List<Träningspass> HämtaAllaTräningspass()
        {
            return _unitOfWork.TräningspassRepository.GetAll().ToList();
        }

    }
}
