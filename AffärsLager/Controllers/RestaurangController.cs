using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class RestaurangController
    {
        private readonly UnitOfWork _unitOfWork;

        public RestaurangController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

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