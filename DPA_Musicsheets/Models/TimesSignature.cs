using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class TimesSignature: MusicalSymbol
    {
        public int numberOfBeats;
        public int beatsPerBar;

        public TimesSignature()
        {

        }

        public TimesSignature(IMidiMessage metaMessage)
        {
            byte[] timeSignatureBytes = metaMessage.GetBytes();
            numberOfBeats = timeSignatureBytes[0];
            beatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));
        }

        public override string toLilyPond()
        {
            return $"\\time {numberOfBeats}/{beatsPerBar}";
        }
    }
}
