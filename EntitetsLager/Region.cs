using System.ComponentModel.DataAnnotations;

namespace EntitetsLager
{
    public class Region
    {
        [Key]
        public int RegionID { get; set; }

        [Required]
        [StringLength(50)]
        public string Regionnamn { get; set; }

        public int AntalRestauranger { get; set; }
    }
}