using System.ComponentModel.DataAnnotations;

namespace EntitetsLager
{
    public class Meny
    {
        [Key]
        public int MenyID { get; set; }

        [Required]
        [StringLength(100)]
        public string Rattnamn { get; set; }

        [StringLength(500)]
        public string Beskrivning { get; set; }

        [Required]
        public decimal Pris { get; set; }

        [Required]
        [StringLength(50)]
        public string Kategori { get; set; } // Mat, Alkoholhaltig dryck, Alkoholfri dryck

        public bool ArGrundmeny { get; set; } = true;

        public bool Aktiv { get; set; } = true;
    }
}