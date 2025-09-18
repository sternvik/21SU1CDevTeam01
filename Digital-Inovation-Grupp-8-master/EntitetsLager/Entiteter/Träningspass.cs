using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitetsLager.Entiteter
{
    public class Träningspass
    {
        [Key]
        public int TräningspassID { get; set; }
        public int Distans { get; set; }
        public string Tempo { get; set; } 
        public DateTime Datum { get; set; }
        public TimeSpan Tid { get; set; }
        public string Plats { get; set; }
        public string Beskrivning { get; set; }
        public int? DeltagarAntal { get; set; }
        public int? MaxDeltagare { get; set; }
        public ICollection<MedlemTräningspass> MedlemTräningspass { get; set; }
    }
}
