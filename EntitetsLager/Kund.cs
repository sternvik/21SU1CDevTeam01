using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitetsLager
{
    public class Kund
    {
        [Key]
        public int KundID { get; set; }

        [Required]
        [StringLength(100)]
        public string Namn { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Telefon { get; set; }

        public int? RegionID { get; set; }

        public int? HemmarestaurangID { get; set; }

        public int LojalitetsPoang { get; set; } = 0;

        [StringLength(10)]
        public string LojalitetsNiva { get; set; } = "Brons"; // Brons, Silver, Guld

        public DateTime SkapadDatum { get; set; } = DateTime.Now;

        public virtual Region Region { get; set; }
        public virtual Restaurang Hemmarestaurang { get; set; }

        public virtual ICollection<Bokning> Bokningar { get; set; } = new List<Bokning>();
        public virtual ICollection<Bestallning> Bestallningar { get; set; } = new List<Bestallning>();
        public virtual ICollection<LojalitetsTransaktion> LojalitetsTransaktioner { get; set; } = new List<LojalitetsTransaktion>();
    }
}
