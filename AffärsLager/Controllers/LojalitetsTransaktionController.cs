using EntitetsLager;
using DataLager;
using System;
using System.Linq;

namespace AffärsLager.Controllers
{
    public class LojalitetsTransaktionController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();
        private KundController _kundController = new KundController();

        /// <summary>
        /// Hämtar kundens aktuella poängsaldo
        /// </summary>
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
        /// Tilldelar lojalitetspoäng till kund och uppdaterar nivå
        /// Middag = 15p, Lunch = 10p, Avhämtning = 10p
        /// </summary>
        public bool TilldelaPoang(int kundId, int poang, int? bestallningsId = null, string beskrivning = "Betalning")
        {
            try
            {
                // Hämta kunden direkt från samma UnitOfWork för att säkerställa att ändringar sparas
                var kund = _unitOfWork.KundRepository.FirstOrDefault(k => k.KundID == kundId);
                if (kund == null)
                    throw new InvalidOperationException("Kunden finns inte");

                // Kontrollera om kunden har e-post (krav för poäng)
                if (string.IsNullOrWhiteSpace(kund.Email))
                {
                    // Ingen e-post = ingen poäng enligt krav
                    return false;
                }

                // Hämta nuvarande saldo
                int tidrigareSaldo = kund.LojalitetsPoang;
                int nyttSaldo = tidrigareSaldo + poang;

                // Skapa lojalitetstransaktion
                var transaktion = new LojalitetsTransaktion
                {
                    KundID = kundId,
                    BestallningsID = bestallningsId,
                    PoangTillagda = poang,
                    PoangAnvanda = 0,
                    PoangSaldo = nyttSaldo,
                    Datum = DateTime.Now
                };

                _unitOfWork.LojalitetsTransaktionRepository.Add(transaktion);

                // Uppdatera kund
                kund.LojalitetsPoang = nyttSaldo;
                kund.LojalitetsNiva = BeraknaLojalitetsNiva(nyttSaldo);

                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid tilldelning av poäng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Använder lojalitetspoäng (drar av från saldo)
        /// </summary>
        public bool AnvandPoang(int kundId, int poang, int? bestallningsId = null, string beskrivning = "Poäng använd")
        {
            try
            {
                // Hämta kunden direkt från samma UnitOfWork för att säkerställa att ändringar sparas
                var kund = _unitOfWork.KundRepository.FirstOrDefault(k => k.KundID == kundId);
                if (kund == null)
                    throw new InvalidOperationException("Kunden finns inte");

                if (kund.LojalitetsPoang < poang)
                    throw new InvalidOperationException($"Kunden har bara {kund.LojalitetsPoang} poäng, kan inte använda {poang} poäng");

                // Hämta nuvarande saldo
                int tidrigareSaldo = kund.LojalitetsPoang;
                int nyttSaldo = tidrigareSaldo - poang;

                // Skapa lojalitetstransaktion
                var transaktion = new LojalitetsTransaktion
                {
                    KundID = kundId,
                    BestallningsID = bestallningsId,
                    PoangTillagda = 0,
                    PoangAnvanda = poang,
                    PoangSaldo = nyttSaldo,
                    Datum = DateTime.Now
                };

                _unitOfWork.LojalitetsTransaktionRepository.Add(transaktion);

                // Uppdatera kund
                kund.LojalitetsPoang = nyttSaldo;
                kund.LojalitetsNiva = BeraknaLojalitetsNiva(nyttSaldo);

                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fel vid användning av poäng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Beräknar lojalitetsnivå baserat på poäng
        /// Brons: 0-39p, Silver: 40-74p, Guld: 75+p
        /// </summary>
        private string BeraknaLojalitetsNiva(int poang)
        {
            if (poang >= 75)
                return "Guld";
            else if (poang >= 40)
                return "Silver";
            else
                return "Brons";
        }

        /// <summary>
        /// Hämtar alla transaktioner för en kund
        /// </summary>
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
        /// Kräver minst 100 poäng
        /// </summary>
        public bool KanAnvandaPoangForBetalning(int kundId)
        {
            var poang = HamtaKundsPoang(kundId);
            return poang >= 100;
        }
    }
}