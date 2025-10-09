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

        public List<Meny> SokMeny(string? rattnamn = null, string? kategori = null)
        {
            try
            {
                var query = _unitOfWork.MenyRepository.GetAll().Where(m => m.Aktiv);

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

                // Kontrollera om rättnamnet redan finns
                var befintlig = _unitOfWork.MenyRepository.FirstOrDefault(m =>
                    m.Rattnamn.ToLower() == meny.Rattnamn.ToLower() && m.Aktiv);

                if (befintlig != null)
                    throw new InvalidOperationException("En meny med detta rättnamn finns redan");

                meny.Aktiv = true;
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

                // Soft delete (markera som inaktiv)
                meny.Aktiv = false;

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