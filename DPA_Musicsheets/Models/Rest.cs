
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;

namespace DPA_Musicsheets.Models
{
    class Rest: Token
    {
        public string duration;

        public Rest(string duration)
        {
            this.duration = duration;
        }

        public int Duration => (int)(1 / float.Parse(duration));

        public void AcceptLily(LilyVisitor v)
        {
            v.Visit(this);
        }
    }
}
