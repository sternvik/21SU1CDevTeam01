using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    /// <summary>
    /// Bestallning - Mat- och dryckesbeställning
    /// Kan vara för Middag (med bokning), Lunch (utan bokning), Avhämtning eller Utkörning
    /// Innehåller alla rätter kunden beställt via BestallningsRader
    /// </summary>
    public class Bestallning
    {
        /// <summary>Unikt ID för beställningen (Primary Key)</summary>
        [Key]
        public int BestallningsID { get; set; }

        /// <summary>
        /// Koppling till bokning (Foreign Key till Bokning)
        /// NULL för lunch (lunch kräver ingen bokning)
        /// Fylld för middagar (kopplas till bordsbokning)
        /// </summary>
        public int? BokningsID { get; set; }

        /// <summary>Vilken kund som beställt (Foreign Key till Kund)</summary>
        [Required]
        public int KundID { get; set; }

        /// <summary>På vilken restaurang beställningen är (Foreign Key till Restaurang)</summary>
        [Required]
        public int RestaurangID { get; set; }

        /// <summary>Vilken personal som tog beställningen (Foreign Key till Anvandare)</summary>
        [Required]
        public int AnvandarID { get; set; }

        /// <summary>
        /// Typ av beställning:
        /// - Middag: Kopplad till bokning, 15 poäng
        /// - Lunch: Ingen bokning, 10 poäng
        /// - Avhämtning: Take-away, 10 poäng
        /// - Utkörning: Leverans via Foodora/Wolt, 0 poäng
        /// </summary>
        [Required]
        [StringLength(20)]
        public string BestallningsTyp { get; set; }

        /// <summary>Utkörningsservice för leveranser (t.ex. Foodora, Wolt)</summary>
        [StringLength(50)]
        public string Utkorare { get; set; }

        /// <summary>Total summa för hela beställningen (exkl. dricks)</summary>
        [Required]
        public decimal TotalSumma { get; set; }

        /// <summary>Om beställningen är betald eller inte</summary>
        public bool Betald { get; set; } = false;

        /// <summary>
        /// Lojalitetspoäng att tilldela:
        /// - Middag: 15 poäng
        /// - Lunch: 10 poäng
        /// - Avhämtning: 10 poäng
        /// - Utkörning: 0 poäng
        /// </summary>
        public int PoangTilldelas { get; set; }

        /// <summary>Dricks (tips) från kunden - är momsbefriad</summary>
        public decimal Dricks { get; set; } = 0;

        /// <summary>Datum för beställningen</summary>
        [Required]
        public DateTime Datum { get; set; }

        /// <summary>Tid för beställningen</summary>
        [Required]
        public TimeSpan Tid { get; set; }

        // Navigation Properties (används av Entity Framework för relationer)
        /// <summary>Bokningen som beställningen tillhör (null för lunch)</summary>
        public virtual Bokning Bokning { get; set; }

        /// <summary>Kunden som beställt</summary>
        public virtual Kund Kund { get; set; }

        /// <summary>Restaurangen där beställningen är</summary>
        public virtual Restaurang Restaurang { get; set; }

        /// <summary>Personalen som tog beställningen</summary>
        public virtual Anvandare AnvandareBeh { get; set; }

        /// <summary>Alla enskilda rätter i beställningen (t.ex. 2x Pizza, 1x Pasta)</summary>
        public virtual ICollection<BestallningsRad> BestallningsRader { get; set; } = new List<BestallningsRad>();

        /// <summary>Transaktion för bokföring (skapas när beställningen betalas)</summary>
        public virtual ICollection<Transaktion> Transaktioner { get; set; } = new List<Transaktion>();
    }
}