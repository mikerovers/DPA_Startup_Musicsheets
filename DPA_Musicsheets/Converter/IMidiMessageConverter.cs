using DPA_Musicsheets.Converter.Midi;
using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter
{
    interface IMidiMessageConverter
    {
        void Parse(IMidiMessage message, MidiEvent midiEvent, MidiConverterMetaData metaData, Block block);
    }
}
