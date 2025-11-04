using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// RegionController - Hanterar geografiska regioner
    /// Systemet har 4 regioner: Norr, Öst, Väst, Syd
    /// </summary>
    public class RegionController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        /// <summary>
        /// Hämtar alla regioner i systemet
        /// Returnerar 4 regioner: Norr (2 rest), Öst (7 rest), Väst (5 rest), Syd (4 rest)
        /// </summary>
        /// <returns>Lista med alla regioner sorterade på namn</returns>
        public List<Region> HamtaAllaRegioner()
        {
            try
            {
                return _unitOfWork.RegionRepository.GetAll()
                    .OrderBy(r => r.Regionnamn)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av regioner: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hämtar en specifik region baserat på ID
        /// </summary>
        /// <param name="regionId">Regionens ID (1-4)</param>
        /// <returns>Regionen om den finns, annars null</returns>
        public Region? HamtaRegionMedId(int regionId)
        {
            try
            {
                return _unitOfWork.RegionRepository.FirstOrDefault(r =>
                    r.RegionID == regionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av region med ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hämtar en region baserat på namn
        /// Sökning är case-insensitive (Norr = norr = NORR)
        /// </summary>
        /// <param name="regionnamn">Regionens namn</param>
        /// <returns>Regionen om den finns, annars null</returns>
        public Region? HamtaRegionMedNamn(string regionnamn)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regionnamn))
                    return null;

                return _unitOfWork.RegionRepository.FirstOrDefault(r =>
                    r.Regionnamn.ToLower() == regionnamn.Trim().ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av region med namn: {ex.Message}", ex);
            }
        }
    }
}
