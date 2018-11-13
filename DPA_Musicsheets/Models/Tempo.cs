using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;

namespace DPA_Musicsheets.Models
{
    class Tempo : Token
    {
        public int tempo;

        public Tempo(int tempo)
        {
            this.tempo = tempo;
        }

        public void AcceptLily(LilyVisitor v)
        {
            v.Visit(this);
        }
    }
}
