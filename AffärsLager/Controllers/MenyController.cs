using EntitetsLager;
using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// MenyController - Hanterar menyvaror (rätter och drycker)
    /// Ansvarar för grundmenyn och restaurangspecifika menyer
    /// </summary>
    public class MenyController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        /// <summary>
        /// Hämtar alla menyvaror i grundmenyn
        /// Sorterar på kategori (Mat, Alkohol, Alkoholfritt) och sedan rättnamn
        /// </summary>
        /// <returns>Lista med alla menyvaror</returns>
        public List<Meny> HamtaAllaMenyvaror()
        {
            try
            {
                // Hämta ALLA menyer (vi använder permanent delete, inte soft delete)
                return _unitOfWork.MenyRepository.GetAll()
                    .OrderBy(m => m.Kategori)
                    .ThenBy(m => m.Rattnamn)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av menyvaror: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hämtar menyvaror som är tillgängliga på en specifik restaurang
        /// Inkluderar både grundmeny och restaurangspecifika rätter
        /// </summary>
        /// <param name="restaurangId">Restaurangens ID</param>
        /// <returns>Lista med menyvaror för restaurangen</returns>
        public List<Meny> HamtaMenyvarorForRestaurang(int restaurangId)
        {
            try
            {
                // Hämta BARA menyer som har RestaurangMeny-koppling för denna restaurang
                // Använd join för att undvika problem med navigation properties
                var menyIds = _unitOfWork.RestaurangMenyRepository.GetAll()
                    .Where(rm => rm.RestaurangID == restaurangId)
                    .Select(rm => rm.MenyID)
                    .ToList();

                var menyer = _unitOfWork.MenyRepository.GetAll()
                    .Where(m => menyIds.Contains(m.MenyID))
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

        /// <summary>
        /// Hämtar en specifik menyvaror baserat på ID
        /// </summary>
        /// <param name="menyId">Menyvarans ID</param>
        /// <returns>Menyn om den finns, annars null</returns>
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

        /// <summary>
        /// Söker efter menyvaror baserat på rättnamn och/eller kategori
        /// Används för att hitta specifika rätter i admin-gränssnitt
        /// </summary>
        /// <param name="rattnamn">Rättnamn att söka efter (valfritt)</param>
        /// <param name="kategori">Kategori att filtrera på (valfritt)</param>
        /// <returns>Lista med matchande menyvaror</returns>
        public List<Meny> SokMeny(string? rattnamn = null, string? kategori = null)
        {
            try
            {
                var query = _unitOfWork.MenyRepository.GetAll().AsQueryable();

                if (!string.IsNullOrWhiteSpace(rattnamn))
                    query = query.Where(m => m.Rattnamn.ToLower().Contains(rattnamn.Trim().ToLower()));

                if (!string.IsNullOrWhiteSpace(kategori))
                    query = query.Where(m => m.Kategori.ToLower().Contains(kategori.Trim().ToLower()));

                return query.OrderBy(m => m.Kategori)
                            .ThenBy(m => m.Rattnamn)
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid sökning av meny: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Skapar en ny menyvaror i grundmenyn
        /// Validerar att rättnamn, pris och kategori finns
        /// </summary>
        /// <param name="meny">Menyobjekt med alla uppgifter</param>
        /// <returns>True om menyn skapades</returns>
        public bool SkapaMeny(Meny meny)
        {
            try
            {
                if (meny == null)
                    throw new ArgumentNullException(nameof(meny));

                if (string.IsNullOrWhiteSpace(meny.Rattnamn))
                    throw new ArgumentException("Rättnamn är obligatoriskt");

                if (meny.Pris <= 0)
                    throw new ArgumentException("Pris måste vara större än 0");

                if (string.IsNullOrWhiteSpace(meny.Kategori))
                    throw new ArgumentException("Kategori är obligatoriskt");

                // Olika restauranger kan ha samma rättnamn - ingen validering
                _unitOfWork.MenyRepository.Add(meny);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid skapande av meny: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Uppdaterar en befintlig menyvaror
        /// Kan ändra rättnamn, beskrivning, pris, kategori och aktiv-status
        /// </summary>
        /// <param name="meny">Menyobjekt med nya uppgifter</param>
        /// <returns>True om uppdateringen lyckades</returns>
        public bool UppdateraMeny(Meny meny)
        {
            try
            {
                if (meny == null)
                    throw new ArgumentNullException(nameof(meny));

                var befintlig = HamtaMenyMedId(meny.MenyID);
                if (befintlig == null)
                    throw new InvalidOperationException("Menyn finns inte i databasen");

                // Uppdatera alla fält
                befintlig.Rattnamn = meny.Rattnamn;
                befintlig.Beskrivning = meny.Beskrivning;
                befintlig.Pris = meny.Pris;
                befintlig.Kategori = meny.Kategori;
                befintlig.ArGrundmeny = meny.ArGrundmeny;
                befintlig.Aktiv = meny.Aktiv;

                _unitOfWork.Save();
                _unitOfWork.RefreshContext(); // Rensa EF cache för att tvinga färska data vid nästa hämtning
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid uppdatering av meny: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tar bort en menyvaror permanent från databasen
        /// OBS: Detta påverkar alla restauranger som har rätten
        /// </summary>
        /// <param name="menyId">ID för menyn som ska tas bort</param>
        /// <returns>True om borttagningen lyckades</returns>
        public bool TaBortMeny(int menyId)
        {
            try
            {
                var meny = HamtaMenyMedId(menyId);
                if (meny == null)
                    throw new InvalidOperationException("Menyn finns inte i databasen");

                // Permanent borttagning från databasen
                _unitOfWork.MenyRepository.Remove(meny);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid borttagning av meny: {ex.Message}", ex);
            }
        }
    }
}
