using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Bokning
    {
        [Key]
        public int BokningsID { get; set; }

        [Required]
        [ForeignKey("Kund")]
        public int KundID { get; set; }

        [Required]
        [ForeignKey("Bord")]
        public int BordID { get; set; }

        [Required]
        [ForeignKey("Restaurang")]
        public int RestaurangID { get; set; }

        [ForeignKey("Anvandare")]
        public int? AnvandarID { get; set; }

        [Required]
        public DateTime Datum { get; set; }

        [Required]
        public TimeSpan Tid { get; set; }

        public int AntalGaster { get; set; }

        [StringLength(500)]
        public string Specialinformation { get; set; }

        [StringLength(20)]
        public string BokningsTyp { get; set; } // Telefon, Online, På plats

        [StringLength(20)]
        public string Status { get; set; } = "Bokad"; // Bokad, Bekräftad, Avslutad, Avbokad

        public DateTime SkapadDatum { get; set; } = DateTime.Now;

        // public virtual Kund Kund { get; set; }
        // public virtual Bord Bord { get; set; }
        // public virtual Restaurang Restaurang { get; set; }
        // public virtual Anvandare Anvandare { get; set; }
    }
}