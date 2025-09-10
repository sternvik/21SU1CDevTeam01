using System.ComponentModel.DataAnnotations;

namespace EntitetsLager
{
    public class Kund
    {
        [Key]
        public int KundID { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
