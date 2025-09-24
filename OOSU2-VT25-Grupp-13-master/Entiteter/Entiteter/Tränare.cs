using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitetsLager
{
    //Tränare-entitet
    public class Tränare
    {
        [Key]
        public int TränareID { get; set; }
        public string Namn { get; set; }
        public string Specialisering { get; set; }
        public string Lösenord { get; set; }

        public ICollection<Träningspass> Träningspass { get; set; }
    }
}
