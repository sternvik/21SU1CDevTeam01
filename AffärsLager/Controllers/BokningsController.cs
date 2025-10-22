using AffärsLager.Services;
using DataLager;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class BokningsController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();
        private LoggService _loggService = new LoggService();

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

        public List<BordMedStatus> HamtaAllaBordMedStatus(int restaurangId, DateTime datum, TimeSpan tid, int antalGaster)
        {
            try
            {
                // Hämta alla bord för restaurangen
                var allaBord = _unitOfWork.BordRepository.GetAll()
                    .Where(b => b.RestaurangID == restaurangId)
                    .ToList();

                // Hämta alla bokningar för valt datum och tid
                var vadTidSlut = tid.Add(TimeSpan.FromHours(2));
                var bokningar = _unitOfWork.BokningRepository.GetAll()
                    .Where(b => b.RestaurangID == restaurangId &&
                               b.Datum.Date == datum.Date &&
                               b.Status != "Avbokad" && b.Status != "Avslutad")
                    .ToList()
                    .Where(b =>
                    {
                        var bokningSlut = b.Tid.Add(TimeSpan.FromHours(2));
                        // Kollision: deras start < vårt slut OCH deras slut > vårt start
                        return b.Tid < vadTidSlut && bokningSlut > tid;
                    })
                    .ToList();

                return allaBord.Select(bord =>
                {
                    var bokning = bokningar.FirstOrDefault(b => b.BordID == bord.BordID);
                    var arLedigt = bokning == null;
                    var bordStatus = "Ledigt";

                    if (bokning != null)
                    {
                        if (bokning.Status == "Betalt")
                            bordStatus = "Betalt";
                        else if (bokning.Status == "På plats")
                            bordStatus = "På plats";
                        else
                            bordStatus = "Bokat";
                    }

                    return new BordMedStatus
                    {
                        BordID = bord.BordID,
                        Bordkod = bord.Bordkod,
                        AntalPlatser = bord.AntalPlatser,
                        ArLedigt = arLedigt,
                        ArLampligt = bord.AntalPlatser >= antalGaster,
                        BokadTid = bokning?.Tid,
                        BordStatus = bordStatus,
                        BokningsID = bokning?.BokningsID,
                        Bokning = bokning
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fel vid hämtning av bord med status: {ex.Message}", ex);
            }
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
                var vadTidSlut = tid.Add(TimeSpan.FromHours(2));
                var bokadeBord = _unitOfWork.BokningRepository.GetAll()
                    .Where(b => b.RestaurangID == restaurangId &&
                               b.Datum.Date == datum.Date &&
                               b.Status != "Avbokad")
                    .ToList()
                    .Where(b =>
                    {
                        var bokningSlut = b.Tid.Add(TimeSpan.FromHours(2));
                        // Kollision: deras start < vårt slut OCH deras slut > vårt start
                        return b.Tid < vadTidSlut && bokningSlut > tid;
                    })
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

                // Kontrollera att bordet är ledigt - använd korrekt antal gäster för färglogik men tillåt alla bokningar
                var bordMedStatus = HamtaAllaBordMedStatus(bokning.RestaurangID, bokning.Datum, bokning.Tid, bokning.AntalGaster);
                var valdtBord = bordMedStatus.FirstOrDefault(b => b.BordID == bokning.BordID);

                if (valdtBord == null)
                    throw new InvalidOperationException($"Bord med ID {bokning.BordID} finns inte på restaurang {bokning.RestaurangID}");

                if (!valdtBord.ArLedigt)
                    throw new InvalidOperationException("Valt bord är inte ledigt under den önskade tiden");

                if (!valdtBord.ArLampligt)
                    throw new InvalidOperationException($"Bordet har bara {valdtBord.AntalPlatser} platser men {bokning.AntalGaster} gäster ska placeras");

                // Sätt standardvärden
                bokning.Status = "Bokad";
                bokning.SkapadDatum = DateTime.Now;
                bokning.BokningsTyp = string.IsNullOrWhiteSpace(bokning.BokningsTyp) ? "På plats" : bokning.BokningsTyp;

                // Spara bokningen
                _unitOfWork.BokningRepository.Add(bokning);

                // Uppdatera bordstatus till "Bokat"
                var bord = _unitOfWork.BordRepository.FirstOrDefault(b => b.BordID == bokning.BordID);
                if (bord != null)
                {
                    bord.Status = "Bokat";
                }

                _unitOfWork.Save();

                // Logga bokning
                var kund = _unitOfWork.KundRepository.GetQuery().FirstOrDefault(k => k.KundID == bokning.KundID);
                _loggService.LoggaHandelse(
                    bokning.AnvandarID ?? 0,
                    "Bokning",
                    $"Skapade bokning #{bokning.BokningsID} för {kund?.Namn ?? "Okänd kund"}",
                    $"Bord: {bord?.Bordkod ?? "Okänt"}, Datum: {bokning.Datum:yyyy-MM-dd}, Tid: {bokning.Tid:hh\\:mm}, Gäster: {bokning.AntalGaster}");

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

        public Bokning? HamtaBokningForBord(int bordId, DateTime datum, TimeSpan tid)
        {
            try
            {
                var vadTidSlut = tid.Add(TimeSpan.FromHours(2));
                return _unitOfWork.BokningRepository.GetAll()
                    .Where(b => b.BordID == bordId &&
                               b.Datum.Date == datum.Date &&
                               b.Status != "Avbokad" &&
                               b.Status != "Avslutad")  // Filtrera även bort avslutade bokningar
                    .ToList()
                    .FirstOrDefault(b =>
                    {
                        var bokningSlut = b.Tid.Add(TimeSpan.FromHours(2));
                        return b.Tid < vadTidSlut && bokningSlut > tid;
                    });
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av bokning för bord: {ex.Message}", ex);
            }
        }

        public bool CheckInBokning(int bokningsId, int anvandarId)
        {
            try
            {
                var bokning = HamtaBokningMedId(bokningsId);
                if (bokning == null)
                    throw new InvalidOperationException("Bokningen finns inte");

                if (bokning.Status == "På plats")
                    throw new InvalidOperationException("Bokningen är redan incheckad");

                if (bokning.Status != "Bokad")
                    throw new InvalidOperationException($"Kan inte checka in bokning med status: {bokning.Status}");

                bokning.Status = "På plats";

                // Uppdatera bordstatus till "Aktivt"
                var bord = _unitOfWork.BordRepository.FirstOrDefault(b => b.BordID == bokning.BordID);
                if (bord != null)
                {
                    bord.Status = "Aktivt";
                }

                _unitOfWork.Save();

                // Logga check-in
                var kund = _unitOfWork.KundRepository.GetQuery().FirstOrDefault(k => k.KundID == bokning.KundID);
                _loggService.LoggaHandelse(
                    anvandarId,
                    "Bokning",
                    $"Check-in bokning #{bokningsId} för {kund?.Namn ?? "Okänd kund"}",
                    $"Datum: {bokning.Datum:yyyy-MM-dd}, Tid: {bokning.Tid:hh\\:mm}");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid check-in av bokning: {ex.Message}", ex);
            }
        }

        public bool CheckOutBokning(int bokningsId, int anvandarId)
        {
            try
            {
                var bokning = HamtaBokningMedId(bokningsId);
                if (bokning == null)
                    throw new InvalidOperationException("Bokningen finns inte");

                if (bokning.Status != "På plats" && bokning.Status != "Betalt")
                    throw new InvalidOperationException("Bokningen måste vara incheckad eller betald för att kunna checkas ut");

                bokning.Status = "Avslutad";

                // Uppdatera bordstatus till "Ledigt"
                var bord = _unitOfWork.BordRepository.FirstOrDefault(b => b.BordID == bokning.BordID);
                if (bord != null)
                {
                    bord.Status = "Ledigt";
                }

                _unitOfWork.Save();

                // Logga check-out
                var kund = _unitOfWork.KundRepository.GetQuery().FirstOrDefault(k => k.KundID == bokning.KundID);
                _loggService.LoggaHandelse(
                    anvandarId,
                    "Bokning",
                    $"Check-out bokning #{bokningsId} för {kund?.Namn ?? "Okänd kund"}",
                    $"Datum: {bokning.Datum:yyyy-MM-dd}, Tid: {bokning.Tid:hh\\:mm}");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid check-out av bokning: {ex.Message}", ex);
            }
        }

        public bool AvbokaBokning(int bokningsId, int anvandarId)
        {
            try
            {
                var bokning = HamtaBokningMedId(bokningsId);
                if (bokning == null)
                    throw new InvalidOperationException("Bokningen finns inte");

                if (bokning.Status == "Avbokad")
                    throw new InvalidOperationException("Bokningen är redan avbokad");

                if (bokning.Status == "Avslutad")
                    throw new InvalidOperationException("Kan inte avboka en avslutad bokning");

                if (bokning.Status == "På plats")
                    throw new InvalidOperationException("Kan inte avboka en bokning där kunden redan är incheckad");

                if (bokning.Status != "Bokad")
                    throw new InvalidOperationException($"Kan inte avboka bokning med status: {bokning.Status}");

                bokning.Status = "Avbokad";

                _unitOfWork.Save();

                // Logga avbokning
                var kund = _unitOfWork.KundRepository.GetQuery().FirstOrDefault(k => k.KundID == bokning.KundID);
                _loggService.LoggaHandelse(
                    anvandarId,
                    "Bokning",
                    $"Avbokade bokning #{bokningsId} för {kund?.Namn ?? "Okänd kund"}",
                    $"Datum: {bokning.Datum:yyyy-MM-dd}, Tid: {bokning.Tid:hh\\:mm}");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid avbokning: {ex.Message}", ex);
            }
        }
    }

    public class BordMedStatus
    {
        public int BordID { get; set; }
        public string Bordkod { get; set; } = string.Empty;
        public int AntalPlatser { get; set; }
        public bool ArLedigt { get; set; }
        public bool ArLampligt { get; set; }
        public TimeSpan? BokadTid { get; set; }
        public string BordStatus { get; set; } = "Ledigt"; // Ledigt, Bokat, På plats
        public int? BokningsID { get; set; }
        public Bokning? Bokning { get; set; }
    }
}