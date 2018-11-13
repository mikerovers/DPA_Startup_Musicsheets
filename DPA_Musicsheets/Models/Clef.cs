using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;

namespace DPA_Musicsheets.Models
{
    class Clef : Token
    {
        public ClefType type;
        public int bar;

        public Clef(ClefType type)
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
