using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Bestallning
    {
        [Key]
        public int BestallningsID { get; set; }

        [ForeignKey("Bokning")]
        public int? BokningsID { get; set; } // null för lunch

        [Required]
        [ForeignKey("Kund")]
        public int KundID { get; set; }

        [Required]
        [ForeignKey("Restaurang")]
        public int RestaurangID { get; set; }

        [Required]
        [ForeignKey("Anvandare")]
        public int AnvandarID { get; set; }

        [Required]
        [StringLength(20)]
        public string BestallningsTyp { get; set; } // Middag, Lunch, Avhämtning, Utkörning

        [StringLength(50)]
        public string Utkorare { get; set; } // för avhämtning - t.ex. Foodora, Wolt

        [Required]
        public decimal TotalSumma { get; set; }

        public bool Betald { get; set; } = false;

        public int PoangTilldelas { get; set; } // 10 lunch/avhämtning, 15 middag

        [Required]
        public DateTime Datum { get; set; }

        [Required]
        public TimeSpan Tid { get; set; }

        // public virtual Bokning Bokning { get; set; }
        // public virtual Kund Kund { get; set; }
        // public virtual Restaurang Restaurang { get; set; }
        // public virtual Anvandare Anvandare { get; set; }
    }
}