using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLager;
using EntitetsLager;

namespace AffärsLager
{
    public class MedlemController
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork();//Hanterar transaktioner och repositories.


        /// <summary>
        /// Lägger till en ny medlem om det inte redan finns en medlem med samma namn, telefonummer eller epost.
        /// </summary>
        /// <param name="medlem"> Medlemsobjektet som ska läggas till.  </param>
        /// <returns> sträng medelande om operationen lyckas eller misslyckas. </returns>
        public string LäggTillMedlem(Medlem medlem)
        {
            if (!ÄrMedlemUnik(medlem.Namn, medlem.Telefonnummer, medlem.Epost))
            {
                return "Det finns redan en medlem med samma namn, telefonnummer eller e-post.";
            }

            _unitOfWork.MedlemRepository.Add(medlem);
            _unitOfWork.Save();

            return "Medlem har lagts till!";
        }

        /// <summary>
        /// Uppdaterar en befintlig medlems uppgifter.
        /// </summary>
        /// <param name="medlemattUppdatera"> Medlemsobjekt med uppdaterade uppgifter.</param>
        public void UppdateraMedlem(Medlem medlemattUppdatera)
        {
            var medlem = _unitOfWork.MedlemRepository.FirstOrDefault(m => m.MedlemID == medlemattUppdatera.MedlemID);
            if (medlem != null)
            {
                //uppdaterar medlemsinformation för befintlig medlem.
                medlem.Namn = medlemattUppdatera.Namn;
                medlem.Telefonnummer = medlemattUppdatera.Telefonnummer;
                medlem.Födelse = medlemattUppdatera.Födelse;
                medlem.Epost = medlemattUppdatera.Epost;
                medlem.Betalstatus = medlemattUppdatera.Betalstatus;

                _unitOfWork.Save(); // Spara ändringar i databasen.
            }
        }

        /// <summary>
        /// Tar bort en medlem från systemet.
        /// </summary>
        /// <param name="medlemattTabort"> Medlemsobjektet som ska tas bort. </param>
        public void TaBortMedlem(Medlem medlemattTabort)
        {
            var medlem = _unitOfWork.MedlemRepository.FirstOrDefault(m => m.MedlemID == medlemattTabort.MedlemID);
            if (medlem != null)
            {
                _unitOfWork.MedlemRepository.Remove(medlem); // Tar bort medlemmen.
                _unitOfWork.Save(); // Spara ändringar i databasen.
            }
        }

        /// <summary>
        /// Visar Betalstatus för en medlem.
        /// </summary>
        /// <param name="medlemID"> ID för den medlem vars betalstatus ska hämtas. </param>
        /// <returns>True om medlemmen har betalat annars false. </returns>
        public bool VisaMedlemStatus(int medlemID)
        {
            var medlem = _unitOfWork.MedlemRepository.FirstOrDefault(m => m.MedlemID == medlemID);
            if (medlem != null)
            {
                return medlem.Betalstatus;
            }
            return false;
        }

        /// <summary>
        /// Kontrollerar om en medlem är unik baserat på namn, telefonummer eller epost
        /// Används innan medlem läggs till.
        /// </summary>
        /// <param name="namn"> Medlemmens namn.</param>
        /// <param name="telefonnummer"> Medlemmens telefonnummer.</param>
        /// <param name="epost"> Medlemmens epost.</param>
        /// <returns>True om medlem är unik annars false.</returns>
        public bool ÄrMedlemUnik(string namn, string telefonnummer, string epost)
        {
            var medlem = _unitOfWork.MedlemRepository.FirstOrDefault(m => m.Namn == namn || m.Telefonnummer == telefonnummer || m.Epost == epost);

            return medlem == null;
        }

        /// <summary>
        /// Hämtar en lista med alla medlemmar i systemet.
        /// </summary>
        /// <returns> En lista innehållande alla medlemmar.</returns>
        public IEnumerable<Medlem> HämtaAllaMedlemmar()
        {
            return _unitOfWork.MedlemRepository.GetAll();
        }
    }
}
