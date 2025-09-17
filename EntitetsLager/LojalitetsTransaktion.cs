using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class LojalitetsTransaktion
    {
        [Key]
        public int LojalitetsTransaktionsID { get; set; }

        [Required]
        [ForeignKey("Kund")]
        public int KundID { get; set; }

        [ForeignKey("Bestallning")]
        public int? BestallningsID { get; set; }

        public int PoangTillagda { get; set; } = 0;

        public int PoangAnvanda { get; set; } = 0;

        [Required]
        public int PoangSaldo { get; set; }

        [Required]
        public DateTime Datum { get; set; }

        // public virtual Kund Kund { get; set; }
        // public virtual Bestallning Bestallning { get; set; }
    }
}