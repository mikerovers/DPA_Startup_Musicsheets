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
    class ChannelMessageConverter : IMidiMessageConverter
    {
        MidiConverterMetaData metaData;
        Block block;

        public void Parse(IMidiMessage message, MidiEvent midiEvent, MidiConverterMetaData metaData, Block block)
        {
            this.block = block;
            this.metaData = metaData;
            var channelMessage = (ChannelMessage)message;

            if (channelMessage.Data2 > 0)
            {
                HandleNoteOn(channelMessage, midiEvent);
            }
            else if (!metaData.startedNoteIsClosed)
            {
                HandleNoteOff(channelMessage, midiEvent);
            }
        }

        public void HandleNoteOn(ChannelMessage channelMessage, MidiEvent e)
        {
            metaData.CurNote = new Note();
            metaData.CurNote.key = MidiToLilyHelper.GetLilyNoteName(metaData.previousMidiKey, channelMessage.Data1);
            metaData.previousMidiKey = channelMessage.Data1;
            metaData.startedNoteIsClosed = false;
        }

        public void HandleNoteOff(ChannelMessage channelMessage, MidiEvent e)
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
