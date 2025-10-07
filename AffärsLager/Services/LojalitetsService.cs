using System;

namespace AffärsLager.Services
{
    /// <summary>
    /// Centraliserad service för lojalitetsprogrammet
    /// Hanterar nivåer, poängberäkning och förmåner
    /// </summary>
    public static class LojalitetsService
    {
        // Tröskelvärden för lojalitetsnivåer
        public const int BRONS_MIN = 0;
        public const int SILVER_MIN = 40;
        public const int GULD_MIN = 75;

        // Poäng per aktivitet
        public const int POANG_MIDDAG = 15;
        public const int POANG_LUNCH = 10;
        public const int POANG_AVHAMTNING = 10;

        // Poängkrav för förmåner
        public const int POANG_FOR_GRATIS_LUNCH = 100;
        public const int POANG_FOR_RABATT = 50;

        /// <summary>
        /// Beräknar lojalitetsnivå baserat på poäng
        /// Brons: 0-39p, Silver: 40-74p, Guld: 75+p
        /// </summary>
        public static string BeraknaLojalitetsNiva(int poang)
        {
            return poang switch
            {
                >= GULD_MIN => "Guld",
                >= SILVER_MIN => "Silver",
                _ => "Brons"
            };
        }

        /// <summary>
        /// Kontrollerar om kund kan få gratis lunch (kräver 100p)
        /// </summary>
        public static bool KanFaGratisLunch(int poang)
        {
            return poang >= POANG_FOR_GRATIS_LUNCH;
        }

        /// <summary>
        /// Kontrollerar om kund kan använda poäng för rabatt (kräver 50p)
        /// </summary>
        public static bool KanAnvandaPoangForRabatt(int poang)
        {
            return poang >= POANG_FOR_RABATT;
        }

        /// <summary>
        /// Beräknar rabatt baserat på lojalitetsnivå
        /// Brons: 5%, Silver: 10%, Guld: 15%
        /// </summary>
        public static decimal BeraknaRabattProcent(string niva)
        {
            return niva switch
            {
                "Guld" => 0.15m,
                "Silver" => 0.10m,
                "Brons" => 0.05m,
                _ => 0m
            };
        }

        /// <summary>
        /// Beräknar rabattbelopp baserat på nivå och totalpris
        /// </summary>
        public static decimal BeraknaRabattBelopp(string niva, decimal totalpris)
        {
            return totalpris * BeraknaRabattProcent(niva);
        }

        /// <summary>
        /// Beräknar hur många poäng som krävs för nästa nivå
        /// </summary>
        public static int PoangTillNastaNiva(int nuvarandePoang)
        {
            if (nuvarandePoang < SILVER_MIN)
                return SILVER_MIN - nuvarandePoang;
            else if (nuvarandePoang < GULD_MIN)
                return GULD_MIN - nuvarandePoang;
            else
                return 0; // Redan på högsta nivån
        }

        /// <summary>
        /// Returnerar beskrivning av förmåner för en specifik nivå
        /// </summary>
        public static string HamtaFormanerBeskrivning(string niva)
        {
            return niva switch
            {
                "Guld" => "15% rabatt på alla beställningar, gratis lunch vid 100p",
                "Silver" => "10% rabatt på alla beställningar, gratis lunch vid 100p",
                "Brons" => "5% rabatt på alla beställningar, gratis lunch vid 100p",
                _ => "Inga förmåner"
            };
        }
    }
}
