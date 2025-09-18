using DataLager.DataLager.Datalager;
using EntitetsLager.Entiteter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLager.DataLager;
using DataLager.DataLager.Datalager;
using EntitetsLager.Entiteter;

namespace AffärsLager.Controllers
{
    public class MedlemController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public string LäggTillMedlem(Medlem medlem)
        {
            if (!ÄrMedlemUnik(medlem.Namn, medlem.Telefonnummer, medlem.Epost))
            {
                return "Det finns redan en medlem med samma namn, telefonnummer eller e-post.";
            }

            _unitOfWork.MedlemsRepository.Add(medlem);
            _unitOfWork.Save();

            return "Medlem har lagts till!";
        }

        public bool ÄrMedlemUnik(string namn, string telefonnummer, string epost)
        {
            var medlem = _unitOfWork.MedlemsRepository.FirstOrDefault(m => m.Namn == namn || m.Telefonnummer == telefonnummer || m.Epost == epost);

            return medlem == null;
        }

        public List<string> HämtaKön()
        {
            return new List<string>
            {
                "Man",
                "Kvinna",
                "Icke binär"
            };
        }

        public void HämtaAllaMedlemmar()
        {
            _unitOfWork.MedlemsRepository.GetAll();
        }

        public void TaBortMedlem(int medlemsID)
        {
            var medlem = _unitOfWork.MedlemsRepository.FirstOrDefault(m => m.MedlemID == medlemsID);
            if (medlem != null)
            {
                _unitOfWork.MedlemsRepository.Remove(medlem);
                _unitOfWork.Save();
            }
            else
            {
                throw new Exception("Medlem inte hittad.");
            }
        }
    }
}
