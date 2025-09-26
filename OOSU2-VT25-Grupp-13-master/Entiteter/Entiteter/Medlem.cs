using EntitetsLager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitetsLager
{
    // Medlem-entitet.
    public class Medlem
    {
        [Key]
        public int MedlemID { get; set; }
        public string Namn { get; set; }
        public string Telefonnummer { get; set; }
        public DateTime Födelse { get; set; }
        public string Epost { get; set; }
        public bool Betalstatus { get; set; }
        public string Lösenord { get; set; }
        public int Poäng {  get; set; }
        public int Kalorier { get; set; }


        public ICollection<MedlemTräningspass> MedlemTräningspass { get; set; }
        public ICollection<Utlåning> Utlåningar { get; set; }
    }
}
