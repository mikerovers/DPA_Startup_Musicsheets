using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class Note : Token
    {
        public Pitch pitch;
        public int octave;
        public Key key;
        public Division length;

        public Note(Pitch pitch, int octave, Division length, Key key)
        {
            this.length = length;
            this.key = key;
            this.octave = octave;
            this.pitch = pitch;
        }

    }
}
