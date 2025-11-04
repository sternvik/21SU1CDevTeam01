using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// BordController - Hanterar bordinformation
    /// Ansvarar för att hämta bord för en restaurang
    /// </summary>
    public class BordController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        /// <summary>
        /// Hämtar alla bord för en specifik restaurang
        /// Sorterar först på antal platser, sedan på bordkod
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <returns>Lista med bord sorterade på kapacitet</returns>
        public List<Bord> HamtaBordForRestaurang(int restaurangId)
        {
            try
            {
                return _unitOfWork.BordRepository.GetAll()
                    .Where(b => b.RestaurangID == restaurangId)
                    .OrderBy(b => b.AntalPlatser)
                    .ThenBy(b => b.Bordkod)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av bord för restaurang: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hämtar ett specifikt bord baserat på ID
        /// Används för att få bordets information och status
        /// </summary>
        /// <param name="bordId">Bordets ID</param>
        /// <returns>Bordet om det finns, annars null</returns>
        public Bord? HamtaBordMedId(int bordId)
        {
            try
            {
                return _unitOfWork.BordRepository.FirstOrDefault(b => b.BordID == bordId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av bord med ID: {ex.Message}", ex);
            }
        }
    }
}
