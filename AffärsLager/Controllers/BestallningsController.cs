using AffärsLager.Services;
using EntitetsLager;
using DataLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class BestallningsController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();
        private LoggService _loggService = new LoggService();

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

        public Bestallning SkapaEllerUppdateraBestallning(int? bokningsId, int kundId, int restaurangId, int anvandarId,
            List<BestallningsRadDto> bestallningsrader, string bestallningsTyp = "Middag", string? utkorare = null, decimal dricks = 0, bool betald = true)
        {
            try
            {
                // Hämta befintlig beställning eller skapa ny
                Bestallning? befintligBestallning = null;

                if (bokningsId.HasValue)
                {
                    befintligBestallning = HamtaBefintligBestallningForBokning(bokningsId.Value);
                }

                if (befintligBestallning == null)
                {
                    // Bestäm poäng baserat på typ
                    int poangTilldelas = bestallningsTyp switch
                    {
                        "Lunch" => 10,
                        "Avhämtning" => 10,
                        "Middag" => 15,
                        _ => 0
                    };

                    // Skapa ny beställning
                    befintligBestallning = new Bestallning
                    {
                        BokningsID = bokningsId,
                        KundID = kundId,
                        RestaurangID = restaurangId,
                        AnvandarID = anvandarId,
                        BestallningsTyp = bestallningsTyp,
                        Utkorare = utkorare ?? "", // Sätt tom sträng istället för null
                        TotalSumma = 0,
                        Betald = false,
                        PoangTilldelas = poangTilldelas,
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

                // Uppdatera totalsumma, dricks och betald-status
                befintligBestallning.TotalSumma = totalSumma;
                befintligBestallning.Dricks = dricks;
                befintligBestallning.Betald = betald;

                // Om beställningen är betald och kopplad till en bokning, uppdatera bordstatus till "Betalt"
                if (betald && befintligBestallning.BokningsID.HasValue)
                {
                    var bokning = _unitOfWork.BokningRepository.FirstOrDefault(b => b.BokningsID == befintligBestallning.BokningsID.Value);
                    if (bokning != null)
                    {
                        var bord = _unitOfWork.BordRepository.FirstOrDefault(b => b.BordID == bokning.BordID);
                        if (bord != null)
                        {
                            bord.Status = "Betalt";
                        }
                    }
                }

                _unitOfWork.Save();

                // Logga beställning
                var kund = _unitOfWork.KundRepository.GetQuery().FirstOrDefault(k => k.KundID == kundId);
                _loggService.LoggaHandelse(
                    anvandarId,
                    "Beställning",
                    $"Registrerade beställning #{befintligBestallning.BestallningsID} för {kund?.Namn ?? "Okänd kund"}",
                    $"Typ: {bestallningsTyp}, Summa: {totalSumma:C}, Dricks: {dricks:C}, Antal rader: {bestallningsrader.Count}");

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