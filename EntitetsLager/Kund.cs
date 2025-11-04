using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    /// <summary>
    /// Kund - Representerar en kund i RestoNation-systemet
    /// Kunder är centraliserade och kan besöka vilken restaurang som helst
    /// Lojalitetsprogrammet är kopplat till kunden
    /// </summary>
    public class Kund
    {
        /// <summary>Unikt ID för kunden (Primary Key)</summary>
        [Key]
        public int KundID { get; set; }

        /// <summary>Kundens fullständiga namn (obligatoriskt)</summary>
        [Required]
        [StringLength(100)]
        public string Namn { get; set; }

        /// <summary>Email för marknadsföring och kampanjer (valfritt)</summary>
        [StringLength(100)]
        public string? Email { get; set; }

        /// <summary>Telefonnummer - används för kundsökning (valfritt men rekommenderat)</summary>
        [StringLength(20)]
        public string? Telefon { get; set; }

        /// <summary>Vilken region kunden tillhör (Foreign Key till Region)</summary>
        public int? RegionID { get; set; }

        /// <summary>Kundens föredragna restaurang (Foreign Key till Restaurang)</summary>
        public int? HemmarestaurangID { get; set; }

        /// <summary>Aktuellt lojalitetspoängsaldo (startar på 0)</summary>
        public int LojalitetsPoang { get; set; } = 0;

        /// <summary>Lojalitetsnivå: Brons (0-39p), Silver (40-74p), Guld (75+p)</summary>
        [StringLength(10)]
        public string LojalitetsNiva { get; set; } = "Brons";

        /// <summary>När kunden registrerades i systemet</summary>
        public DateTime SkapadDatum { get; set; } = DateTime.Now;

        // Navigation Properties (används av Entity Framework för relationer)
        /// <summary>Region som kunden tillhör</summary>
        public virtual Region Region { get; set; }

        /// <summary>Kundens hemmarestaurang</summary>
        public virtual Restaurang Hemmarestaurang { get; set; }

        /// <summary>Alla bokningar kunden har gjort</summary>
        public virtual ICollection<Bokning> Bokningar { get; set; } = new List<Bokning>();

        /// <summary>Alla beställningar kunden har gjort</summary>
        public virtual ICollection<Bestallning> Bestallningar { get; set; } = new List<Bestallning>();

        /// <summary>Historik över lojalitetspoängtransaktioner</summary>
        public virtual ICollection<LojalitetsTransaktion> LojalitetsTransaktioner { get; set; } = new List<LojalitetsTransaktion>();
    }
}
