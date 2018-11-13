using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Managers;
using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Converter.Midi
{
    class MetaMessageConverter : IMidiMessageConverter
    {
        MidiConverterMetaData metaData;
        Block block;

        public void Parse(IMidiMessage message, MidiEvent midiEvent, MidiConverterMetaData metaData, Block block)
        {
            this.metaData = metaData;
            this.block = block;
            var metaMessage = (MetaMessage)message;

            switch (metaMessage.MetaType)
            {
                case MetaType.TimeSignature:
                    HandleTimeSignature(metaMessage, midiEvent);

                    break;
                case MetaType.Tempo:
                    HandleTempo(metaMessage, midiEvent);

                    break;
                case MetaType.EndOfTrack:
                    HandleEndOfTrack(metaMessage, midiEvent);

                    break;
            }
        }

        public TimeSignature HandleTimeSignature(MetaMessage metaMessage, MidiEvent e)
        {
            byte[] timeSignatureBytes = metaMessage.GetBytes();
            metaData._beatNote = timeSignatureBytes[0];
            metaData._beatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));

            var signature = new Models.TimeSignature(metaData._beatNote, metaData._beatsPerBar);
            block.Add(signature);

            return signature;
        }

        public Tempo HandleTempo(MetaMessage metaMessage, MidiEvent e)
        {
            byte[] tempoBytes = metaMessage.GetBytes();
            int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);

            var tempoModel = new Models.Tempo(tempo);
            block.Add(tempoModel);

            return tempoModel;
        }

        public void HandleEndOfTrack(MetaMessage metaMessage, MidiEvent e)
        {
            if (metaData.previousNoteAbsoluteTicks > 0)
            {
                double percentageOfBar;
                string length = MidiToLilyHelper.GetLilypondNoteLength(metaData.previousNoteAbsoluteTicks, e.AbsoluteTicks, metaData.division, metaData._beatNote, metaData._beatsPerBar, out percentageOfBar);
                metaData.CurNote.length = length;

                metaData.previousNoteAbsoluteTicks = e.AbsoluteTicks;
                metaData.percentageOfBarReached += percentageOfBar;
                metaData.startedNoteIsClosed = true;

                block.Add(metaData.CurNote);
            }
        }
    }
}
