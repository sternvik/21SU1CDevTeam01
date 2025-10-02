using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class RegionController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

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