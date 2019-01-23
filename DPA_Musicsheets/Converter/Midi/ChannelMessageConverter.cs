using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DPA_Musicsheets.Managers;
using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Converter.Midi
{
    class ChannelMessageConverter : AbstractMidiMessageConverter
    {
        MidiConverterMetaData metaData;
        Block block;

        public override void Parse(IMidiMessage message, MidiEvent midiEvent, MidiConverterMetaData metaData, Block block)
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
            metaData.CurNote.key = GetKey(channelMessage, metaData.previousMidiKey);
            metaData.CurNote.octave = (channelMessage.Data1 / 12) - 1;
            metaData.CurNote.pitch = GetPitch(GetLilyNoteName(metaData.previousMidiKey, channelMessage.Data1));
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

            var value = GetLilypondNoteLength(metaData.previousNoteAbsoluteTicks, e.AbsoluteTicks, metaData.division, metaData._beatNote, metaData._beatsPerBar, out percentageOfBar);
            var length = float.Parse(Regex.Match(value, @"[0-9]+").Value);
            var dotAmount = value.Count(i => i.Equals('.'));

            var totalLenght = (1 / length) * (2 - 1 / Math.Pow(2, dotAmount));
            metaData.CurNote.length = "" + totalLenght;

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

        private Pitch GetPitch(string name)
        {
            switch (name.Substring(0,1).ToLower())
            {
                case "c":
                    return Pitch.C;
                case "d":
                    return Pitch.D;
                case "e":
                    return Pitch.E;
                case "f":
                    return Pitch.F;
                case "g":
                    return Pitch.G;
                case "a":
                    return Pitch.A;
                case "b":
                    return Pitch.B;
                default:
                    return Pitch.C;
            }
        }  
    }
}
