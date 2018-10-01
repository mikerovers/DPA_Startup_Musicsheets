using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class TempoSignature : MusicalSymbol
    {
        public int bpm;

        public TempoSignature(MetaMessage metaMessage)
        {
            byte[] tempoBytes = metaMessage.GetBytes();
            int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);
            this.bpm = 60000000 / tempo;
        }

        public override string toLilyPond()
        {
            return $"\\tempo 4={this.bpm}";
        }
    }
}
