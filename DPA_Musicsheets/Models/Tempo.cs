using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class Tempo : Token
    {
        public int tempo;

        public Tempo(int tempo)
        {
            this.tempo = tempo;
        }
    }
}
