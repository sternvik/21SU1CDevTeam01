using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Anvandare
    {
        [Key]
        public int AnvandarID { get; set; }

        [Required]
        [StringLength(50)]
        public string Anvandarnamn { get; set; }

        [Required]
        [StringLength(255)]
        public string Losenord { get; set; }

        [Required]
        [StringLength(100)]
        public string Namn { get; set; }

        public int? HemmarestaurangID { get; set; }

        [Required]
        [StringLength(20)]
        public string Roll { get; set; } // Servitör, Admin, Restaurangchef, VD

        public bool Aktiv { get; set; } = true;

        public virtual Restaurang Hemmarestaurang { get; set; }
    }
}