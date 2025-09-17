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
    }
}