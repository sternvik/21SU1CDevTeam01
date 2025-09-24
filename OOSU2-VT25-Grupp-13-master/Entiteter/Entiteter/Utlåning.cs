using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitetsLager
{
    //Utlåning-Entitet
    public class Utlåning
    {
        [Key]
        public int UtlåningID { get; set; }
        public int MedlemID { get; set; }
        public int UtrustningID { get; set; }
        public DateTime UtLåningsdatum { get; set; }
        public DateTime? Återlämningsdatum { get; set; }

        public Medlem Medlem { get; set; }
        public Utrustning Utrustning { get; set; }
    }
}
