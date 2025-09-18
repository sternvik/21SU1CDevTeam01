using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitetsLager.Entiteter
{
    public class MedlemTräningspass
    {
        public int MedlemID { get; set; }
        public Medlem Medlem { get; set; }

        public int TräningspassID { get; set; }
        public Träningspass Träningspass { get; set; }

        public string Status { get; set; }
    }
}
