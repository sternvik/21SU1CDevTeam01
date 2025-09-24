using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Bestallning
    {
        [Key]
        public int BestallningsID { get; set; }

        public int? BokningsID { get; set; } // null för lunch

        [Required]
        public int KundID { get; set; }

        [Required]
        public int RestaurangID { get; set; }

        [Required]
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

        public virtual Bokning Bokning { get; set; }
        public virtual Kund Kund { get; set; }
        public virtual Restaurang Restaurang { get; set; }
        public virtual Anvandare AnvandareBeh { get; set; }

        public virtual ICollection<BestallningsRad> BestallningsRader { get; set; }
        public virtual ICollection<Transaktion> Transaktioner { get; set; }
    }
}