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
            } else
            {
                block.Add(new Rest(""));  
            }
        }

        public void HandleNoteOn(ChannelMessage channelMessage, MidiEvent e)
        {
            metaData.CurNote = new Note();
            // TODO Change key to domain model
            // metaData.CurNote.key = MidiToLilyHelper.GetLilyNoteName(metaData.previousMidiKey, channelMessage.Data1);
            metaData.CurNote.key = GetKey(channelMessage, metaData.previousMidiKey);
            metaData.previousMidiKey = channelMessage.Data1;
            metaData.startedNoteIsClosed = false;
        }

        private Key GetKey(ChannelMessage channelMessage, int previousMidiKey)
        {
            var midiKey = channelMessage.Data1;
            var rest = midiKey % 12;
            Key key;
            if (rest > 4)
            {
                rest = rest - 5;
            }

            if (rest % 2 == 0)
            {
                key = new Key(KeyType.NORM, 1);
            }
            else
            {
                key = new Key(KeyType.SHARP, 1);
            }

            return key;
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

            if (metaData.percentageOfBarReached >= 1)
            {
                block.Add(new Bar());
                metaData.percentageOfBarReached -= 1;
            }
        }
    }
}
