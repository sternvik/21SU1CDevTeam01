using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitetsLager
{
    // Klass som representerar en koppling mellan Medlem och Träningspass.
    // Den används för att hantera många-till-många-relationen mellan medlemmar och träningspass.
    public class MedlemTräningspass
    {
        public int MedlemID { get; set; }
        public Medlem Medlem { get; set; }

        public int TräningspassID { get; set; }
        public Träningspass Träningspass { get; set; }

        public string Status { get; set; }
    }
}
