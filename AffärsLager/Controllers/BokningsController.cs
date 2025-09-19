using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class BokningsController
    {
        private readonly UnitOfWork _unitOfWork;

        public BokningsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public List<TimeSpan> HamtaTillgangligaTider()
        {
            // Tillgängliga tider: 16:00-21:00 (sista bokning 21:00 för 2h slot till 23:00)
            return new List<TimeSpan>
            {
                new TimeSpan(16, 0, 0), // 16:00-18:00
                new TimeSpan(17, 0, 0), // 17:00-19:00
                new TimeSpan(18, 0, 0), // 18:00-20:00
                new TimeSpan(19, 0, 0), // 19:00-21:00
                new TimeSpan(20, 0, 0), // 20:00-22:00
                new TimeSpan(21, 0, 0)  // 21:00-23:00
            };
        }

        public List<Bord> HamtaLedigaBord(int restaurangId, DateTime datum, TimeSpan tid, int antalGaster)
        {
            try
            {
                // Hämta alla bord för restaurangen som kan ta minst antalGaster
                var allaBord = _unitOfWork.BordRepository.GetAll()
                    .Where(b => b.RestaurangID == restaurangId && b.AntalPlatser >= antalGaster)
                    .ToList();

                // Kontrollera vilka bord som är lediga under den önskade tiden (2 timmar)
                var bokadeBord = _unitOfWork.BokningRepository.GetAll()
                    .Where(b => b.RestaurangID == restaurangId &&
                               b.Datum.Date == datum.Date &&
                               b.Status != "Avbokad" &&
                               // Kontrollera överlappning med 2-timmars slot
                               ((b.Tid <= tid && b.Tid.Add(TimeSpan.FromHours(2)) > tid) ||
                                (b.Tid < tid.Add(TimeSpan.FromHours(2)) && b.Tid.Add(TimeSpan.FromHours(2)) >= tid.Add(TimeSpan.FromHours(2)))))
                    .Select(b => b.BordID)
                    .ToList();

                return allaBord.Where(b => !bokadeBord.Contains(b.BordID))
                              .OrderBy(b => b.AntalPlatser) // Föreslå minsta lämpliga bord först
                              .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av lediga bord: {ex.Message}", ex);
            }
        }

        public bool SkapaBokning(Bokning bokning)
        {
            try
            {
                if (bokning == null)
                    throw new ArgumentNullException(nameof(bokning));

                // Validera obligatoriska fält
                if (bokning.KundID <= 0)
                    throw new ArgumentException("Kund måste anges");

                if (bokning.BordID <= 0)
                    throw new ArgumentException("Bord måste anges");

                if (bokning.RestaurangID <= 0)
                    throw new ArgumentException("Restaurang måste anges");

                if (bokning.AntalGaster <= 0)
                    throw new ArgumentException("Antal gäster måste vara minst 1");

                // Validera datum och tid
                if (bokning.Datum < DateTime.Today)
                    throw new ArgumentException("Bokningsdatum kan inte vara i det förflutna");

                var tillgangligaTider = HamtaTillgangligaTider();
                if (!tillgangligaTider.Contains(bokning.Tid))
                    throw new ArgumentException("Vald tid är inte tillgänglig för bokning");

                // Kontrollera att bordet är ledigt
                var ledigaBord = HamtaLedigaBord(bokning.RestaurangID, bokning.Datum, bokning.Tid, bokning.AntalGaster);
                if (!ledigaBord.Any(b => b.BordID == bokning.BordID))
                    throw new InvalidOperationException("Valt bord är inte ledigt under den önskade tiden");

                // Sätt standardvärden
                bokning.Status = "Bokad";
                bokning.SkapadDatum = DateTime.Now;
                bokning.BokningsTyp = string.IsNullOrWhiteSpace(bokning.BokningsTyp) ? "På plats" : bokning.BokningsTyp;

                // Spara bokningen
                _unitOfWork.BokningRepository.Add(bokning);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid skapande av bokning: {ex.Message}", ex);
            }
        }

        public List<Bokning> HamtaBokningarForRestaurang(int restaurangId, DateTime? datum = null)
        {
            try
            {
                var query = _unitOfWork.BokningRepository.GetAll()
                    .Where(b => b.RestaurangID == restaurangId && b.Status != "Avbokad");

                if (datum.HasValue)
                {
                    query = query.Where(b => b.Datum.Date == datum.Value.Date);
                }

                return query.OrderBy(b => b.Datum).ThenBy(b => b.Tid).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av bokningar: {ex.Message}", ex);
            }
        }

        public Bokning? HamtaBokningMedId(int bokningsId)
        {
            try
            {
                return _unitOfWork.BokningRepository.FirstOrDefault(b => b.BokningsID == bokningsId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av bokning: {ex.Message}", ex);
            }
        }
    }
}