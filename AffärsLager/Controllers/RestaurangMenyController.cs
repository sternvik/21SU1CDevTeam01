using EntitetsLager;
using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// RestaurangMenyController - Hanterar kopplingen mellan restauranger och menyer
    /// En restaurang kan ha många menyvaror, och en menyvaror kan finnas på många restauranger
    /// Detta är en "many-to-many"-relation som hanteras via RestaurangMeny-tabellen
    /// </summary>
    public class RestaurangMenyController
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        /// <summary>
        /// Hämtar alla menyvaror som är tillgängliga på en specifik restaurang
        /// Inkluderar både grundmeny och restaurangspecifika rätter
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <returns>Lista med menyvaror sorterade på kategori och rättnamn</returns>
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

        /// <summary>
        /// Kopplar en menyvaror till en restaurang
        /// T.ex. för att lägga till en ny rätt på restaurangens meny
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <param name="menyId">Menyvarans ID</param>
        public void KopplaMenyTillRestaurang(int restaurangId, int menyId)
        {
            // Kontrollera om kopplingen redan finns för att undvika dubbletter
            var redanKopplad = _unitOfWork.RestaurangMenyRepository
                .FirstOrDefault(rm => rm.MenyID == menyId && rm.RestaurangID == restaurangId);

            if (redanKopplad == null)
            {
                // Skapa ny koppling mellan restaurang och meny
                _unitOfWork.RestaurangMenyRepository.Add(new RestaurangMeny
                {
                    RestaurangID = restaurangId,
                    MenyID = menyId
                });
                _unitOfWork.Save();
            }
        }

        /// <summary>
        /// Tar bort en menyvaror från en restaurangs meny
        /// OBS: Detta tar bara bort kopplingen, inte själva menyvaror
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <param name="menyId">Menyvarans ID</param>
        public void TaBortMenyFranRestaurang(int restaurangId, int menyId)
        {
            // Hitta kopplingen mellan restaurangen och menyvaror
            var rm = _unitOfWork.RestaurangMenyRepository
                .FirstOrDefault(r => r.MenyID == menyId && r.RestaurangID == restaurangId);
            if (rm != null)
            {
                // Ta bort kopplingen (menyvaror finns kvar i systemet)
                _unitOfWork.RestaurangMenyRepository.Remove(rm);
                _unitOfWork.Save();
            }
        }

        /// <summary>
        /// Tar bort alla kopplingar för en menyvaror från ALLA restauranger
        /// Används när en menyvaror ska tas bort helt från systemet
        /// </summary>
        /// <param name="menyId">Menyvarans ID</param>
        public void TaBortAllaKopplingarForMeny(int menyId)
        {
            // Hitta alla restauranger som har denna menyvaror
            var kopplingar = _unitOfWork.RestaurangMenyRepository
                .GetAll()
                .Where(rm => rm.MenyID == menyId)
                .ToList();

            // Ta bort alla kopplingar
            foreach (var koppling in kopplingar)
            {
                _unitOfWork.RestaurangMenyRepository.Remove(koppling);
            }

            // Spara om det fanns några kopplingar att ta bort
            if (kopplingar.Any())
            {
                _unitOfWork.Save();
            }
        }

        /// <summary>
        /// Räknar hur många restauranger som har en specifik menyvaror
        /// Användbart för att se hur populär en rätt är
        /// </summary>
        /// <param name="menyId">Menyvarans ID</param>
        /// <returns>Antal restauranger som har rätten</returns>
        public int RaknaAntalRestaurangerSomAnvanderMeny(int menyId)
        {
            return _unitOfWork.RestaurangMenyRepository
                .GetAll()
                .Count(rm => rm.MenyID == menyId);
        }
    }
}
