using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    /// <summary>
    /// Bokning - Bordsreservation för middag
    /// OBS: Lunch kräver INGEN bokning, endast middagar (16:00-21:00)
    /// Bokningar är kopplade till ett specifikt bord och tidslot (2 timmar)
    /// </summary>
    public class Bokning
    {
        /// <summary>Unikt ID för bokningen (Primary Key)</summary>
        [Key]
        public int BokningsID { get; set; }

        /// <summary>Vilken kund som gjort bokningen (Foreign Key till Kund)</summary>
        [Required]
        public int KundID { get; set; }

        /// <summary>Vilket bord som är bokat (Foreign Key till Bord)</summary>
        [Required]
        public int BordID { get; set; }

        /// <summary>På vilken restaurang bokningen är (Foreign Key till Restaurang)</summary>
        [Required]
        public int RestaurangID { get; set; }

        /// <summary>Vilken personal som tog bokningen (Foreign Key till Anvandare)</summary>
        public int? AnvandarID { get; set; }

        /// <summary>Vilket datum bokningen gäller (t.ex. 2025-01-15)</summary>
        [Required]
        public DateTime Datum { get; set; }

        /// <summary>Vilken tid bokningen gäller (t.ex. 18:00, 19:00) - 2-timmars slot</summary>
        [Required]
        public TimeSpan Tid { get; set; }

        /// <summary>Antal gäster i sällskapet (måste matcha bordets kapacitet)</summary>
        public int AntalGaster { get; set; }

        /// <summary>Specialinformation (allergier, önskemål, etc.)</summary>
        [StringLength(500)]
        public string? Specialinformation { get; set; }

        /// <summary>Hur bokningen gjordes: Telefon, Online eller På plats</summary>
        [StringLength(20)]
        public string BokningsTyp { get; set; }

        /// <summary>
        /// Bokningsstatus:
        /// - Bokad: Bokning registrerad, kund ej anländ
        /// - Bekräftad: Kund har anlänt, bordet är aktivt
        /// - Avslutad: Måltiden avslutad, betalning klar
        /// - Avbokad: Kund bokade av
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; } = "Bokad";

        /// <summary>När bokningen skapades i systemet</summary>
        public DateTime SkapadDatum { get; set; } = DateTime.Now;

        // Navigation Properties (används av Entity Framework för relationer)
        /// <summary>Kunden som gjort bokningen</summary>
        public virtual Kund Kund { get; set; }

        /// <summary>Bordet som är bokat</summary>
        public virtual Bord Bord { get; set; }

        /// <summary>Restaurangen där bokningen är</summary>
        public virtual Restaurang Restaurang { get; set; }

        /// <summary>Personalen som tog bokningen</summary>
        public virtual Anvandare Anvandare { get; set; }

        /// <summary>Alla beställningar kopplade till denna bokning (vanligtvis 1 st)</summary>
        public virtual ICollection<Bestallning> Bestallningar { get; set; } = new List<Bestallning>();
    }
}