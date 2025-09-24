using System;
using System.Linq;
using DataLager;
using EntitetsLager;
using System.Security.Cryptography;
using System.Text;


namespace AffärsLager
{
    /// <summary>
    /// Hanterar Funktioner för inloggning och Registrering av konto (skapandet av ny tränare.)
    /// </summary>
    public class SäkerhetsController
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork();
        public object LoggedInUser { get; private set; }


        /// <summary>
        /// Försöker logga in en tränare baserat på användarnamn och lösenord.
        /// </summary>
        /// <param name="användarnamn"> Tränarens namn</param>
        /// <param name="lösenord"> Tränarens Lösenord</param>
        /// <returns>True om inloggning lyckas annars false.</returns>
        public bool LoggaIn(string användarnamn, string lösenord)
        {
            var tränare = _unitOfWork.TränareRepository.FirstOrDefault(t => t.Namn.Equals(användarnamn));
            if (tränare != null && ÄrLösenordKorrekt(tränare, lösenord))
            {
                LoggedInUser = tränare;
                return true;
            }

            return false;
        }

        public bool LoggaInMedlem(string användarnamn, string lösenord)
        {
            var medlem = _unitOfWork.MedlemRepository.FirstOrDefault(m => m.Namn.Equals(användarnamn));
            if (medlem != null && ÄrLösenordKorrekt(medlem, lösenord))
            {
                InloggadMedlemSingleton.GetInstance().SetInloggadMedlem(medlem);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skapar en ny tränare och sparar den i databasen
        /// </summary>
        /// <param name="tränareattläggatill"> Tränareobjekt med information om den nya tränaren.</param>   
        public void SkapaTränare(Tränare tränareattläggatill)
        {
            if (ÄrTränareDublett(tränareattläggatill.Namn))
            {
                throw new Exception("En tränare med detta namn finns redan.");
            }

            var tränare = new Tränare
            {
                Namn = tränareattläggatill.Namn,
                Lösenord = tränareattläggatill.Lösenord,
                Specialisering = tränareattläggatill.Specialisering
            };

            _unitOfWork.TränareRepository.Add(tränare); //Lägger till tränare. 
            _unitOfWork.Save(); // Spara ändringar i databasen.
        }

        /// <summary>
        /// Validerar om angivet lösenord är korrekt för en tränare.
        /// </summary>
        /// <param name="tränare"> Tränaren vars lösenord ska kontrolleras.</param>
        /// <param name="lösenord"> Det angivna lösenordet.</param>
        /// <returns> True vid match av lösenord annars false.</returns>
        private bool ÄrLösenordKorrekt(dynamic användare, string lösenord)
        {
            return användare != null && användare.Lösenord == lösenord;
        }

        public Medlem GetLoggedInMember()
        {
            return InloggadMedlemSingleton.GetInstance().GetInloggadMedlem();
        }

        public void LoggaUt()
        {
            InloggadMedlemSingleton.GetInstance().Logout();
        }

        /// <summary>
        /// Kontrollerar om det redan finns en tränare med det angivna namnet.
        /// </summary>
        /// <param name="tränarnamn"> Namnet på tränaren som ska kontrolleras</param>
        /// <returns>True om tränaren redan finns, annars false.</returns>
        private bool ÄrTränareDublett(string tränarnamn)
        {
            return _unitOfWork.TränareRepository.FirstOrDefault(t => t.Namn.Equals(tränarnamn)) != null;
        }

    }
}
