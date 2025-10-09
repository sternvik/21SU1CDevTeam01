using EntitetsLager;
using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class RestaurangMenyController
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public List<Meny> HamtaMenyForRestaurang(int restaurangId)
        {
            // Hämta BARA menyer som har RestaurangMeny-koppling för denna restaurang
            // Använd join för att undvika problem med navigation properties
            var menyIds = _unitOfWork.RestaurangMenyRepository.GetAll()
                .Where(rm => rm.RestaurangID == restaurangId)
                .Select(rm => rm.MenyID)
                .ToList();

            return _unitOfWork.MenyRepository.GetAll()
                .Where(m => menyIds.Contains(m.MenyID))
                .OrderBy(m => m.Kategori)
                .ThenBy(m => m.Rattnamn)
                .ToList();
        }

        public void KopplaMenyTillRestaurang(int restaurangId, int menyId)
        {
            var redanKopplad = _unitOfWork.RestaurangMenyRepository
                .FirstOrDefault(rm => rm.MenyID == menyId && rm.RestaurangID == restaurangId);

            if (redanKopplad == null)
            {
                _unitOfWork.RestaurangMenyRepository.Add(new RestaurangMeny
                {
                    RestaurangID = restaurangId,
                    MenyID = menyId
                });
                _unitOfWork.Save();
            }
        }

        public void TaBortMenyFranRestaurang(int restaurangId, int menyId)
        {
            var rm = _unitOfWork.RestaurangMenyRepository
                .FirstOrDefault(r => r.MenyID == menyId && r.RestaurangID == restaurangId);
            if (rm != null)
            {
                _unitOfWork.RestaurangMenyRepository.Remove(rm);
                _unitOfWork.Save();
            }
        }

        public void TaBortAllaKopplingarForMeny(int menyId)
        {
            var kopplingar = _unitOfWork.RestaurangMenyRepository
                .GetAll()
                .Where(rm => rm.MenyID == menyId)
                .ToList();

            foreach (var koppling in kopplingar)
            {
                _unitOfWork.RestaurangMenyRepository.Remove(koppling);
            }

            if (kopplingar.Any())
            {
                _unitOfWork.Save();
            }
        }

        public int RaknaAntalRestaurangerSomAnvanderMeny(int menyId)
        {
            return _unitOfWork.RestaurangMenyRepository
                .GetAll()
                .Count(rm => rm.MenyID == menyId);
        }
    }
}
