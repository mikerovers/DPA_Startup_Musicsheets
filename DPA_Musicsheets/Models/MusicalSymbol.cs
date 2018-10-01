using System;
using System.Collections.Generic;
using System.Linq;  
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    abstract class MusicalSymbol
    {
        protected SymbolDuration duration;

        abstract public string toLilyPond();
    }
}
