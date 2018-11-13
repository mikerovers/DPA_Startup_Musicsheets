using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;

namespace DPA_Musicsheets.Models
{
    class TimeSignature : Token
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
