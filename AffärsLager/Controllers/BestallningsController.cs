using EntitetsLager;
using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class BestallningsController
    {
        private readonly UnitOfWork _unitOfWork;

        public BestallningsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public Bestallning? HamtaBefintligBestallningForBokning(int bokningsId)
        {
            try
            {
                return _unitOfWork.BestallningRepository.GetAll()
                    .Where(b => b.BokningsID == bokningsId && !b.Betald)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av befintlig beställning: {ex.Message}", ex);
            }
        }

        public Bestallning SkapaEllerUppdateraBestallning(int bokningsId, int kundId, int restaurangId, int anvandarId,
            List<BestallningsRadDto> bestallningsrader)
        {
            try
            {
                // Hämta befintlig beställning eller skapa ny
                var befintligBestallning = HamtaBefintligBestallningForBokning(bokningsId);

                if (befintligBestallning == null)
                {
                    // Skapa ny beställning
                    befintligBestallning = new Bestallning
                    {
                        BokningsID = bokningsId,
                        KundID = kundId,
                        RestaurangID = restaurangId,
                        AnvandarID = anvandarId,
                        BestallningsTyp = "Middag",
                        Utkorare = "", // Sätt tom sträng istället för null
                        TotalSumma = 0,
                        Betald = false,
                        PoangTilldelas = 15, // 15 poäng för middag
                        Datum = DateTime.Today,
                        Tid = DateTime.Now.TimeOfDay
                    };

                    _unitOfWork.BestallningRepository.Add(befintligBestallning);
                    _unitOfWork.Save(); // Spara för att få ID
                }

                // Ta bort alla befintliga rader
                var befintligaRader = _unitOfWork.BestallningsRadRepository.GetAll()
                    .Where(br => br.BestallningsID == befintligBestallning.BestallningsID)
                    .ToList();

                foreach (var rad in befintligaRader)
                {
                    _unitOfWork.BestallningsRadRepository.Remove(rad);
                }

                // Lägg till nya rader
                decimal totalSumma = 0;
                foreach (var rad in bestallningsrader)
                {
                    var nyRad = new BestallningsRad
                    {
                        BestallningsID = befintligBestallning.BestallningsID,
                        MenyID = rad.MenyID,
                        Antal = rad.Antal,
                        Pris = rad.Pris,
                        Summa = rad.Pris * rad.Antal
                    };

                    _unitOfWork.BestallningsRadRepository.Add(nyRad);
                    totalSumma += nyRad.Summa;
                }

                // Uppdatera totalsumma
                befintligBestallning.TotalSumma = totalSumma;

                _unitOfWork.Save();
                return befintligBestallning;
            }
            catch (Exception ex)
            {
                var detaljerat = $"Fel vid skapande/uppdatering av beställning: {ex.Message}";
                if (ex.InnerException != null)
                {
                    detaljerat += $"\n\nInner Exception: {ex.InnerException.Message}";
                    if (ex.InnerException.InnerException != null)
                    {
                        detaljerat += $"\n\nInner Inner Exception: {ex.InnerException.InnerException.Message}";
                    }
                }
                throw new Exception(detaljerat, ex);
            }
        }

        public List<BestallningsRadDto> HamtaBestallningsraderForBokning(int bokningsId)
        {
            try
            {
                var bestallning = HamtaBefintligBestallningForBokning(bokningsId);
                if (bestallning == null)
                    return new List<BestallningsRadDto>();

                var rader = _unitOfWork.BestallningsRadRepository.GetAll()
                    .Where(br => br.BestallningsID == bestallning.BestallningsID)
                    .ToList();

                var result = new List<BestallningsRadDto>();
                foreach (var rad in rader)
                {
                    var meny = _unitOfWork.MenyRepository.FirstOrDefault(m => m.MenyID == rad.MenyID);
                    result.Add(new BestallningsRadDto
                    {
                        MenyID = rad.MenyID,
                        Rattnamn = meny?.Rattnamn ?? "Okänd rätt",
                        Pris = rad.Pris,
                        Antal = rad.Antal
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av beställningsrader: {ex.Message}", ex);
            }
        }

        public bool TaBortBestallning(int bokningsId)
        {
            try
            {
                var bestallning = HamtaBefintligBestallningForBokning(bokningsId);
                if (bestallning == null)
                    return true; // Redan borttagen

                // Ta bort alla rader först
                var rader = _unitOfWork.BestallningsRadRepository.GetAll()
                    .Where(br => br.BestallningsID == bestallning.BestallningsID)
                    .ToList();

                foreach (var rad in rader)
                {
                    _unitOfWork.BestallningsRadRepository.Remove(rad);
                }

                // Ta bort beställningen
                _unitOfWork.BestallningRepository.Remove(bestallning);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid borttagning av beställning: {ex.Message}", ex);
            }
        }
    }

    public class BestallningsRadDto
    {
        public int MenyID { get; set; }
        public string Rattnamn { get; set; } = string.Empty;
        public decimal Pris { get; set; }
        public int Antal { get; set; }
        public decimal Totalpris => Pris * Antal;
    }
}