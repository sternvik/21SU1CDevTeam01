using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Bord
    {
        [Key]
        public int BordID { get; set; }

        [Required]
        public int RestaurangID { get; set; }

        [Required]
        [StringLength(20)]
        public string Bordkod { get; set; }

        public int AntalPlatser { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Ledigt"; // Ledigt, Bokat, Aktivt, Betalt

        public virtual Restaurang Restaurang { get; set; }

        public virtual ICollection<Bokning> Bokningar { get; set; }
    }
}