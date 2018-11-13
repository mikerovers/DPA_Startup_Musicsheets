using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;

namespace DPA_Musicsheets.Models
{
    class Note : Token
    {
        public Pitch pitch;
        public int octave;
        public string key;
        public string length;

        public Note()
        {

        }

        public Note(Pitch pitch, int octave, string length, string key)
        {
            this.length = length;
            this.key = key;
            this.octave = octave;
            this.pitch = pitch;
        }

        public void AcceptLily(LilyVisitor v)
        {
            v.Visit(this);
        }
    }
}
