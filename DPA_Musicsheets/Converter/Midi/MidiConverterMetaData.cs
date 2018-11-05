using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Midi
{
    class MidiConverterMetaData
    {
        public int previousMidiKey = 60;
        public bool startedNoteIsClosed = true;
        public int previousNoteAbsoluteTicks = 0;
        public double percentageOfBarReached = 0;
        public int division;

        public int _beatNote = 4;    // De waarde van een beatnote.
        public int _bpm = 120;       // Aantal beatnotes per minute.
        public int _beatsPerBar;     // Aantal beatnotes per maat.

        public Note CurNote;
    }
}
