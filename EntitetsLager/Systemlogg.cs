using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Systemlogg
    {
        [Key]
        public int LoggID { get; set; }

        [ForeignKey("Anvandare")]
        public int? AnvandarID { get; set; }

        [Required]
        [StringLength(50)]
        public string Modul { get; set; }

        [Required]
        [StringLength(500)]
        public string Handelse { get; set; }

        [Required]
        public DateTime Datum { get; set; }

        [Required]
        public TimeSpan Tid { get; set; }

        [StringLength(45)]
        public string IPAdress { get; set; }

        // public virtual Anvandare Anvandare { get; set; }
    }
}