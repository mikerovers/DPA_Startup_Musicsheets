using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;
using DPA_Musicsheets.Converter.Midi;

namespace DPA_Musicsheets.Models
{
    public class TimeSignature : Token
    {
        public int before;
        public int after;

        public TimeSignature(int before, int after)
        {
            this.before = before;
            this.after = after;
        }

        public void AcceptLily(LilyVisitor v)
        {
            v.Visit(this);
        }
    }
}
