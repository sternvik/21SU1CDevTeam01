using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Restaurang
    {
        [Key]
        public int RestaurangID { get; set; }

        [Required]
        [StringLength(100)]
        public string Restaurangnamn { get; set; }

        [Required]
        public int RegionID { get; set; }

        [StringLength(200)]
        public string Adress { get; set; }

        [StringLength(20)]
        public string Telefon { get; set; }

        [StringLength(50)]
        public string Oppettider { get; set; } = "10:30-23:00";

        public virtual Region Region { get; set; }

        public virtual ICollection<Bord> Bord { get; set; } = new List<Bord>();
        public virtual ICollection<Bokning> Bokningar { get; set; } = new List<Bokning>();
        public virtual ICollection<Bestallning> Bestallningar { get; set; } = new List<Bestallning>();
        public virtual ICollection<Anvandare> Anvandare { get; set; } = new List<Anvandare>();
        public virtual ICollection<RestaurangMeny> RestaurangMenyer { get; set; } = new List<RestaurangMeny>();
    }
}