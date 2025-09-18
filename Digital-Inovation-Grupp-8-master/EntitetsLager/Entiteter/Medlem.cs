using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitetsLager.Entiteter
{
    public class Medlem
    {
        [Key]
        public int MedlemID { get; set; }
        public string Namn { get; set; }
        public string Kön { get; set; }
        public string Telefonnummer { get; set; }
        public DateTime Födelse { get; set; }
        public string Epost { get; set; }
        public string Lösenord { get; set; }

        public ICollection<MedlemTräningspass> MedlemTräningspass { get; set; }
    }
}
