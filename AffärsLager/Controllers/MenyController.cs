using EntitetsLager;
using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class MenyController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public List<Meny> HamtaAllaMenyvaror()
        {
            try
            {
                return _unitOfWork.MenyRepository.GetAll()
                    .Where(m => m.Aktiv)
                    .OrderBy(m => m.Kategori)
                    .ThenBy(m => m.Rattnamn)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av menyvaror: {ex.Message}", ex);
            }
        }

        public List<Meny> HamtaMenyvarorForRestaurang(int restaurangId)
        {
            try
            {
                // Hämta antingen grundmeny eller restaurangspecifik meny
                var menyer = _unitOfWork.MenyRepository.GetAll()
                    .Where(m => m.Aktiv &&
                           (m.ArGrundmeny || m.RestaurangMenyer.Any(rm => rm.RestaurangID == restaurangId)))
                    .OrderBy(m => m.Kategori)
                    .ThenBy(m => m.Rattnamn)
                    .ToList();

                return menyer;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av meny för restaurang: {ex.Message}", ex);
            }
        }

        public Meny? HamtaMenyMedId(int menyId)
        {
            try
            {
                return _unitOfWork.MenyRepository.FirstOrDefault(m => m.MenyID == menyId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av menyvaror: {ex.Message}", ex);
            }
        }
    }
}