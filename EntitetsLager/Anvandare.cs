using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Anvandare
    {

        //hej ny branch 

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

        public virtual ICollection<Bokning> Bokningar { get; set; } = new List<Bokning>();
        public virtual ICollection<Bestallning> Bestallningar { get; set; } = new List<Bestallning>();
        public virtual ICollection<Systemlogg> Systemloggar { get; set; } = new List<Systemlogg>();
    }
}