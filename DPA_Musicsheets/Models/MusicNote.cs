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
        private int octave;
        private NotePitch pitch;
        private int numberOfDots;
    }
}
