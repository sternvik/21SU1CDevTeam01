using DataLager;
using EntitetsLager;
using AffärsLager.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class KundController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public List<Kund> SokKunder(string? telefon = null, string? namn = null, string? email = null)
        {
            try
            {
                var query = _unitOfWork.KundRepository.GetAll();

                if (!string.IsNullOrWhiteSpace(telefon))
                {
                    query = query.Where(k => k.Telefon != null && k.Telefon.Contains(telefon.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(namn))
                {
                    query = query.Where(k => k.Namn != null && k.Namn.ToLower().Contains(namn.Trim().ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    query = query.Where(k => k.Email != null && k.Email.ToLower().Contains(email.Trim().ToLower()));
                }

                return query.OrderBy(k => k.Namn).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid sökning av kunder: {ex.Message}", ex);
            }
        }

        public Kund? HamtaKundMedTelefon(string telefon)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(telefon))
                    return null;

                return _unitOfWork.KundRepository.FirstOrDefault(k =>
                    k.Telefon == telefon.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av kund med telefon: {ex.Message}", ex);
            }
        }

        public Kund? HamtaKundMedId(int kundId)
        {
            try
            {
                return _unitOfWork.KundRepository.FirstOrDefault(k =>
                    k.KundID == kundId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av kund med ID: {ex.Message}", ex);
            }
        }

        public bool SkapaKund(Kund kund)
        {
            try
            {
                if (kund == null)
                    throw new ArgumentNullException(nameof(kund));

                // Validera obligatoriska fält
                if (string.IsNullOrWhiteSpace(kund.Namn))
                    throw new ArgumentException("Namn är obligatoriskt");

                if (string.IsNullOrWhiteSpace(kund.Telefon))
                    throw new ArgumentException("Telefonnummer är obligatoriskt");

                // Kontrollera om telefonnummer redan finns
                var befintligKund = HamtaKundMedTelefon(kund.Telefon);
                if (befintligKund != null)
                    throw new InvalidOperationException("En kund med detta telefonnummer finns redan");

                // Sätt standardvärden
                kund.SkapadDatum = DateTime.Now;
                kund.LojalitetsPoang = kund.LojalitetsPoang == 0 ? 0 : kund.LojalitetsPoang;
                kund.LojalitetsNiva = string.IsNullOrWhiteSpace(kund.LojalitetsNiva) ? "Brons" : kund.LojalitetsNiva;

                // Debug: logga kund info
                System.Diagnostics.Debug.WriteLine($"Skapar kund: {kund.Namn}, {kund.Telefon}, Email: {kund.Email ?? "NULL"}");

                // Lägg till kunden
                _unitOfWork.KundRepository.Add(kund);

                // Debug: logga före save
                System.Diagnostics.Debug.WriteLine("Anropar _unitOfWork.Save()...");
                _unitOfWork.Save();

                // Debug: logga efter save
                System.Diagnostics.Debug.WriteLine("Save() anropet slutfört");

                // Verifiera att kunden sparades genom att söka efter den
                var sparadKund = HamtaKundMedTelefon(kund.Telefon);
                if (sparadKund != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Kund verifierad i databas: {sparadKund.Namn} (ID: {sparadKund.KundID})");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("VARNING: Kund hittades inte i databas efter Save()");
                    throw new Exception("Kunden sparades inte korrekt i databasen");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fel vid skapande av kund: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw new Exception($"Fel vid skapande av kund: {ex.Message}", ex);
            }
        }

        public bool UppdateraKund(Kund kund)
        {
            try
            {
                if (kund == null)
                    throw new ArgumentNullException(nameof(kund));

                var befintligKund = HamtaKundMedId(kund.KundID);
                if (befintligKund == null)
                    throw new InvalidOperationException("Kunden finns inte i databasen");

                // Uppdatera fälten
                befintligKund.Namn = kund.Namn;
                befintligKund.Telefon = kund.Telefon;
                befintligKund.Email = kund.Email;
                befintligKund.RegionID = kund.RegionID;
                befintligKund.HemmarestaurangID = kund.HemmarestaurangID;
                befintligKund.LojalitetsPoang = kund.LojalitetsPoang;
                befintligKund.LojalitetsNiva = kund.LojalitetsNiva;

                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid uppdatering av kund: {ex.Message}", ex);
            }
        }

        public bool TaBortKund(int kundId)
        {
            try
            {
                var kund = HamtaKundMedId(kundId);
                if (kund == null)
                    throw new InvalidOperationException("Kunden finns inte i databasen");

                _unitOfWork.KundRepository.Remove(kund);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid borttagning av kund: {ex.Message}", ex);
            }
        }

        public bool LaggTillLojalitetsPoang(int kundId, int poang)
        {
            try
            {
                var kund = HamtaKundMedId(kundId);
                if (kund == null)
                    throw new InvalidOperationException("Kunden finns inte");

                kund.LojalitetsPoang += poang;

                // Uppdatera lojalitetsnivå baserat på poäng
                kund.LojalitetsNiva = LojalitetsService.BeraknaLojalitetsNiva(kund.LojalitetsPoang);

                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid tillägg av lojalitetspoäng: {ex.Message}", ex);
            }
        }

        public List<Kund> HamtaKunderForRestaurang(int restaurangId)
        {
            try
            {
                return _unitOfWork.KundRepository.GetAll()
                    .Where(k => k.HemmarestaurangID == restaurangId)
                    .OrderBy(k => k.Namn)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av kunder för restaurang: {ex.Message}", ex);
            }
        }

    }
}