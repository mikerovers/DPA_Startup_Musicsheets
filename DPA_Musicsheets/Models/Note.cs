using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;
using DPA_Musicsheets.Converter.Midi;

namespace DPA_Musicsheets.Models
{
    public class Note : Token
    {
        public Pitch pitch;
        public int octave;
        public Key key;
        public string length;

        public Note()
        {

        }

        public Note(Pitch pitch, int octave, string length, Key key)
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

        public int Length => (int) (1 / float.Parse(length));

        public string OctaveString()
        {
            if (octave > 0)
            {
                return $",";
            } else if (octave < 0)
            {
                return $"'";
            } else
            {
                return "";
            }
        }
    }
}
