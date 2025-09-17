using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Transaktion
    {
        [Key]
        public int TransaktionsID { get; set; }

        [Required]
        [ForeignKey("Bestallning")]
        public int BestallningsID { get; set; }

        [Required]
        [ForeignKey("Restaurang")]
        public int RestaurangID { get; set; }

        [Required]
        [ForeignKey("Anvandare")]
        public int AnvandarID { get; set; }

        [Required]
        public DateTime Datum { get; set; }

        [Required]
        public decimal MatSumma { get; set; }

        [Required]
        public decimal AlkoholSumma { get; set; }

        [Required]
        public decimal Moms { get; set; }

        [Required]
        public decimal TotalSumma { get; set; }

        // public virtual Bestallning Bestallning { get; set; }
        // public virtual Restaurang Restaurang { get; set; }
        // public virtual Anvandare Anvandare { get; set; }
    }
}