using AffärsLager.Singleton;
using DataLager.DataLager.Datalager;
using EntitetsLager.Entiteter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AffärsLager.Controllers
{
    public class SäkerhetsController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();


        public bool LoggaInMedlem(string användarnamn, string lösenord)
        {
            var medlem = _unitOfWork.MedlemsRepository.FirstOrDefault(m => m.Namn.Equals(användarnamn));
            if (medlem != null && ÄrLösenordKorrekt(medlem, lösenord))
            {
                InloggadMedlemSingleton.GetInstance().SetInloggadMedlem(medlem);
                return true;
            }
            return false;
        }

        public Medlem GetLoggedInMember()
        {
            return InloggadMedlemSingleton.GetInstance().GetInloggadMedlem();
        }

        public void LoggaUt()
        {
            InloggadMedlemSingleton.GetInstance().Logout();
        }

        private bool ÄrLösenordKorrekt(dynamic användare, string lösenord)
        {
            return användare != null && användare.Lösenord == lösenord;
        }

    }
}
