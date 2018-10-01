using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    enum NotePitch
    {
        SHARP,
        FLAT
    }

    class MusicNote : MusicalSymbol
    {
        public int octave;
        public NotePitch pitch;
        public SymbolDuration duration;
        public int numberOfDots;

        public override string toLilyPond()
        {
            throw new NotImplementedException();
        }
    }
}
