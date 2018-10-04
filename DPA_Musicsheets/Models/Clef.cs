using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class Clef : Token
    {
        public ClefType type;
        public int bar;

        public Clef(ClefType type, int bar)
        {
            this.type = type;
            this.bar = bar;
        }
    }
}
