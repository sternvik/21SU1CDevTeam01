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
            return _unitOfWork.MenyRepository.GetAll()
                .Where(m => m.Aktiv && (m.ArGrundmeny ||
                        m.RestaurangMenyer.Any(rm => rm.RestaurangID == restaurangId)))
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
    }
}
