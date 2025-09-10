using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitetsLager
{
    public class Kund
    {
        [Key]
        public int KundID { get; set; }
        public string Namn { get; set; }
        public string Email { get; set; }
    }
}
