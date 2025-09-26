using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitetsLager
{
    //Utrustning-entitet
    public class Utrustning
    {
        [Key]
        public int UtrustningID { get; set; }
        public string Namn { get; set; }
        public string Kategori { get; set; }
        public string Skick { get; set; }
        public int Tillgängliga { get; set; }

        public ICollection<Utlåning> Utlåningar { get; set; }
    }
}
