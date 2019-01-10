using DPA_Musicsheets.Managers;
using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Midi
{
    class FromMidiConverter : IFromConverter
    {
        private Block block;
        private MidiConverterMetaData metaData;
        private MidiMessageConverterFactory messageConverterFactory;

        public int division;

        public FromMidiConverter()
        {
            block = new Block();
            metaData = new MidiConverterMetaData();
            messageConverterFactory = new MidiMessageConverterFactory();
        }

        public Block ConvertToFromFile(string fileName)
        {
            var sequence = new Sequence();
            sequence.Load(fileName);

            return ConvertTo(sequence);
        }

        public Block ConvertTo(Sequence sequence)
        {
            block = new Block();
            metaData = new MidiConverterMetaData();
            metaData.division = sequence.Division;
            division = sequence.Division;

            block.Add(new Clef(ClefType.treble, 0));
            for (int i = 0; i < sequence.Count(); i++)
            {
                Track track = sequence[i];

                foreach (MidiEvent midiEvent in track.Iterator())
                {
                    IMidiMessage midiMessage = midiEvent.MidiMessage;
                    var converter = messageConverterFactory.GetConverter(midiMessage.MessageType);
                    if (converter != null)
                    {
                        converter.Parse(midiMessage, midiEvent, metaData, block);
                    }
                }
            }

            return block;
        }
    }
}
