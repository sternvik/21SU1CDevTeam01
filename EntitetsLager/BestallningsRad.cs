using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class BestallningsRad
    {
        [Key]
        public int BestallningsRadID { get; set; }

        [Required]
        [ForeignKey("Bestallning")]
        public int BestallningsID { get; set; }

        [Required]
        [ForeignKey("Meny")]
        public int MenyID { get; set; }

        [Required]
        public int Antal { get; set; }

        [Required]
        public decimal Pris { get; set; } // vid beställningstillfället

        [Required]
        public decimal Summa { get; set; }

        // public virtual Bestallning Bestallning { get; set; }
        // public virtual Meny Meny { get; set; }
    }
}