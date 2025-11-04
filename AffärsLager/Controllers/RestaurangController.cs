using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// RestaurangController - Hanterar restauranginformation
    /// Ansvarar för att hämta restauranger baserat på region eller ID
    /// </summary>
    public class RestaurangController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        /// <summary>
        /// Hämtar alla restauranger i systemet
        /// Totalt 18 restauranger över 4 regioner
        /// </summary>
        /// <returns>Lista med alla restauranger sorterade på namn</returns>
        public List<Restaurang> HamtaAllaRestauranger()
        {
            try
            {
                return _unitOfWork.RestaurangRepository.GetAll()
                    .OrderBy(r => r.Restaurangnamn)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av restauranger: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hämtar alla restauranger för en specifik region
        /// T.ex. alla restauranger i region "Norr" eller "Väst"
        /// </summary>
        /// <param name="regionId">Regionens ID (1=Norr, 2=Öst, 3=Väst, 4=Syd)</param>
        /// <returns>Lista med restauranger i regionen</returns>
        public List<Restaurang> HamtaRestaurangerForRegion(int regionId)
        {
            try
            {
                return _unitOfWork.RestaurangRepository.GetAll()
                    .Where(r => r.RegionID == regionId)
                    .OrderBy(r => r.Restaurangnamn)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av restauranger för region: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hämtar en specifik restaurang baserat på ID
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <returns>Restaurangen om den finns, annars null</returns>
        public Restaurang? HamtaRestaurangMedId(int restaurangId)
        {
            try
            {
                return _unitOfWork.RestaurangRepository.FirstOrDefault(r =>
                    r.RestaurangID == restaurangId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av restaurang med ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hämtar en restaurang baserat på namn
        /// Sökning är case-insensitive
        /// </summary>
        /// <param name="restaurangnamn">Restaurangens namn</param>
        /// <returns>Restaurangen om den finns, annars null</returns>
        public Restaurang? HamtaRestaurangMedNamn(string restaurangnamn)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(restaurangnamn))
                    return null;

                return _unitOfWork.RestaurangRepository.FirstOrDefault(r =>
                    r.Restaurangnamn.ToLower() == restaurangnamn.Trim().ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av restaurang med namn: {ex.Message}", ex);
            }
        }
    }
}
