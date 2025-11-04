using EntitetsLager;
using DataLager;
using AffärsLager.Services;
using System;
using System.Linq;

namespace AffärsLager.Controllers
{
    /// <summary>
    /// LojalitetsTransaktionController - Hanterar lojalitetsprogram och poängtransaktioner
    /// Ansvarar för att tilldela poäng, använda poäng, räkna ut rabatter och förmåner
    /// Lojalitetsnivåer: Brons (0-39p), Silver (40-74p), Guld (75+p)
    /// </summary>
    public class LojalitetsTransaktionController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();
        private KundController _kundController = new KundController();

        /// <summary>
        /// Hämtar kundens aktuella poängsaldo
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <returns>Antal poäng kunden har</returns>
        public int HamtaKundsPoang(int kundId)
        {
            try
            {
                var kund = _kundController.HamtaKundMedId(kundId);
                return kund?.LojalitetsPoang ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av kundpoäng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hämtar kundens lojalitetsnivå (Brons, Silver, Guld)
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <returns>Lojalitetsnivå som text: "Brons", "Silver" eller "Guld"</returns>
        public string HamtaKundsNiva(int kundId)
        {
            try
            {
                var kund = _kundController.HamtaKundMedId(kundId);
                return kund?.LojalitetsNiva ?? "Brons";
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av lojalitetsnivå: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tilldelar lojalitetspoäng till en kund och uppdaterar deras lojalitetsnivå
        /// Regler: Middag = 15p, Lunch = 10p, Avhämtning = 10p
        /// OBS: Kund MÅSTE ha e-post för att få poäng!
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <param name="poang">Antal poäng att tilldela (t.ex. 15 för middag)</param>
        /// <param name="bestallningsId">Beställnings-ID (valfritt)</param>
        /// <param name="beskrivning">Beskrivning av varför poäng tilldelas</param>
        /// <returns>True om poäng tilldelades, False om kund saknar e-post</returns>
        public bool TilldelaPoang(int kundId, int poang, int? bestallningsId = null, string beskrivning = "Betalning")
        {
            try
            {
                // Hämta kunden direkt från samma UnitOfWork för att säkerställa att ändringar sparas
                var kund = _unitOfWork.KundRepository.FirstOrDefault(k => k.KundID == kundId);
                if (kund == null)
                    throw new InvalidOperationException("Kunden finns inte");

                // Kontrollera om kunden har e-post (krav för poäng)
                // Detta är ett viktigt affärskrav - ingen e-post = inga poäng!
                if (string.IsNullOrWhiteSpace(kund.Email))
                {
                    // Ingen e-post = ingen poäng enligt krav
                    return false;
                }

                // Räkna ut det nya saldot
                int tidrigareSaldo = kund.LojalitetsPoang;
                int nyttSaldo = tidrigareSaldo + poang;

                // Skapa en lojalitetstransaktion för att logga denna händelse
                // Detta ger oss en historik över alla poängrörelser
                var transaktion = new LojalitetsTransaktion
                {
                    KundID = kundId,
                    BestallningsID = bestallningsId,
                    PoangTillagda = poang,  // Positiva poäng (tillagda)
                    PoangAnvanda = 0,  // Inga poäng användes
                    PoangSaldo = nyttSaldo,  // Det nya totala saldot efter denna transaktion
                    Datum = DateTime.Now
                };

                _unitOfWork.LojalitetsTransaktionRepository.Add(transaktion);

                // Uppdatera kundens saldo och lojalitetsnivå
                kund.LojalitetsPoang = nyttSaldo;
                kund.LojalitetsNiva = LojalitetsService.BeraknaLojalitetsNiva(nyttSaldo);  // Omberäkna nivå

                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid tilldelning av poäng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Använder lojalitetspoäng från kundens saldo (drar av poäng)
        /// T.ex. när kund löser in 100p för en gratis lunch
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <param name="poang">Antal poäng att dra av (t.ex. 100 för gratis lunch)</param>
        /// <param name="bestallningsId">Beställnings-ID (valfritt)</param>
        /// <param name="beskrivning">Beskrivning av vad poängen användes till</param>
        /// <returns>True om poäng drogs av, kastar exception om kunden inte har tillräckligt med poäng</returns>
        public bool AnvandPoang(int kundId, int poang, int? bestallningsId = null, string beskrivning = "Poäng använd")
        {
            try
            {
                // Hämta kunden direkt från samma UnitOfWork för att säkerställa att ändringar sparas
                var kund = _unitOfWork.KundRepository.FirstOrDefault(k => k.KundID == kundId);
                if (kund == null)
                    throw new InvalidOperationException("Kunden finns inte");

                // Kontrollera att kunden har tillräckligt med poäng
                if (kund.LojalitetsPoang < poang)
                    throw new InvalidOperationException($"Kunden har bara {kund.LojalitetsPoang} poäng, kan inte använda {poang} poäng");

                // Räkna ut det nya saldot
                int tidrigareSaldo = kund.LojalitetsPoang;
                int nyttSaldo = tidrigareSaldo - poang;

                // Skapa en lojalitetstransaktion för att logga denna händelse
                var transaktion = new LojalitetsTransaktion
                {
                    KundID = kundId,
                    BestallningsID = bestallningsId,
                    PoangTillagda = 0,  // Inga poäng tillagda
                    PoangAnvanda = poang,  // Negativa poäng (använda)
                    PoangSaldo = nyttSaldo,  // Det nya totala saldot efter denna transaktion
                    Datum = DateTime.Now
                };

                _unitOfWork.LojalitetsTransaktionRepository.Add(transaktion);

                // Uppdatera kundens saldo och lojalitetsnivå
                kund.LojalitetsPoang = nyttSaldo;
                kund.LojalitetsNiva = LojalitetsService.BeraknaLojalitetsNiva(nyttSaldo);  // Omberäkna nivå

                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid användning av poäng: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Hämtar all poänghistorik för en kund
        /// Visar alla tillfällen poäng lagts till eller använts
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <returns>Lista med alla transaktioner, senaste först</returns>
        public System.Collections.Generic.List<LojalitetsTransaktion> HamtaKundsTransaktioner(int kundId)
        {
            try
            {
                return _unitOfWork.LojalitetsTransaktionRepository.GetAll()
                    .Where(t => t.KundID == kundId)
                    .OrderByDescending(t => t.Datum)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid hämtning av transaktioner: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kontrollerar om kund kan använda poäng för betalning
        /// Kräver minst 100 poäng för att få gratis lunch
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <returns>True om kunden har minst 100 poäng</returns>
        public bool KanAnvandaPoangForBetalning(int kundId)
        {
            var poang = HamtaKundsPoang(kundId);
            return LojalitetsService.KanFaGratisLunch(poang);
        }

        /// <summary>
        /// Kontrollerar om kund kan få gratis lunch
        /// Kräver minst 100 poäng
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <returns>True om kunden har minst 100 poäng</returns>
        public bool KanFaGratisLunch(int kundId)
        {
            var poang = HamtaKundsPoang(kundId);
            return LojalitetsService.KanFaGratisLunch(poang);
        }

        /// <summary>
        /// Beräknar rabattbelopp för kund baserat på lojalitetsnivå
        /// Brons = 0% rabatt, Silver = 5% rabatt, Guld = 10% rabatt
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <param name="totalpris">Totalpris före rabatt</param>
        /// <returns>Rabattbelopp i kronor (t.ex. 50 kr på 500 kr med Guld-nivå)</returns>
        public decimal BeraknaRabatt(int kundId, decimal totalpris)
        {
            var kund = _kundController.HamtaKundMedId(kundId);
            if (kund == null)
                return 0m;

            return LojalitetsService.BeraknaRabattBelopp(kund.LojalitetsNiva, totalpris);
        }

        /// <summary>
        /// Hämtar en beskrivning av kundens förmåner baserat på lojalitetsnivå
        /// Visar vad kunden får för sin nivå (rabatter, erbjudanden etc.)
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <returns>Text som beskriver förmånerna</returns>
        public string HamtaFormanerBeskrivning(int kundId)
        {
            var kund = _kundController.HamtaKundMedId(kundId);
            if (kund == null)
                return "Inga förmåner";

            return LojalitetsService.HamtaFormanerBeskrivning(kund.LojalitetsNiva);
        }

        /// <summary>
        /// Beräknar hur många poäng kunden behöver för att nå nästa lojalitetsnivå
        /// T.ex. om kund har 25p (Brons), behöver de 15p till för Silver (40p)
        /// </summary>
        /// <param name="kundId">Kundens ID</param>
        /// <returns>Antal poäng till nästa nivå (0 om redan på högsta nivån Guld)</returns>
        public int PoangTillNastaNiva(int kundId)
        {
            var poang = HamtaKundsPoang(kundId);
            return LojalitetsService.PoangTillNastaNiva(poang);
        }
    }
}