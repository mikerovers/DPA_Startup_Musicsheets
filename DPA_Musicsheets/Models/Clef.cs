using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;
using DPA_Musicsheets.Converter.Midi;

namespace DPA_Musicsheets.Models
{
    public class Clef : Token
    {
        public ClefType type;
        public int bar;

        public Clef(ClefType type, int bar)
        {
            this.type = type;
            this.bar = bar;
        }

        public void AcceptLily(LilyVisitor v)
        {
            v.Visit(this);               
        }
    }
}
