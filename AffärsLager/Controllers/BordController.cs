using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class BordController
    {
        private readonly UnitOfWork _unitOfWork;

        public BordController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

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