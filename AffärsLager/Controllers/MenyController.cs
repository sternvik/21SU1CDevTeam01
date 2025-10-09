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



        public bool UppdateraMeny(Meny meny)
        {
            try
            {
                if (meny == null)
                    throw new ArgumentNullException(nameof(meny));

                var befintlig = HamtaMenyMedId(meny.MenyID);
                if (befintlig == null)
                    throw new InvalidOperationException("Menyn finns inte i databasen");

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