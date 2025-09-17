using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class RestaurangMeny
    {
        [Key]
        public int RestaurangMenyID { get; set; }

        [Required]
        [ForeignKey("Restaurang")]
        public int RestaurangID { get; set; }

        [Required]
        [ForeignKey("Meny")]
        public int MenyID { get; set; }

        public decimal? LokalPris { get; set; }

        public bool Tillganglig { get; set; } = true;

        // public virtual Restaurang Restaurang { get; set; }
        // public virtual Meny Meny { get; set; }
    }
}