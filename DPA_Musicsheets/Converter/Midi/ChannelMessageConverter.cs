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

        private string GetLilyNoteName(int previousMidiKey, int midiKey)
        {
            int octave = (midiKey / 12) - 1;
            string name = "";
            switch (midiKey % 12)
            {
                case 0:
                    name = "c";
                    break;
                case 1:
                    name = "cis";
                    break;
                case 2:
                    name = "d";
                    break;
                case 3:
                    name = "dis";
                    break;
                case 4:
                    name = "e";
                    break;
                case 5:
                    name = "f";
                    break;
                case 6:
                    name = "fis";
                    break;
                case 7:
                    name = "g";
                    break;
                case 8:
                    name = "gis";
                    break;
                case 9:
                    name = "a";
                    break;
                case 10:
                    name = "ais";
                    break;
                case 11:
                    name = "b";
                    break;
            }

            int distance = midiKey - previousMidiKey;
            while (distance < -6)
            {
                name += ",";
                distance += 8;
            }

            while (distance > 6)
            {
                name += "'";
                distance -= 8;
            }

            return name;
        }

        private string GetLilypondNoteLength(int absoluteTicks, int nextNoteAbsoluteTicks, int division, int beatNote, int beatsPerBar, out double percentageOfBar)
        {
            int duration = 0;
            int dots = 0;

            double deltaTicks = nextNoteAbsoluteTicks - absoluteTicks;

            if (deltaTicks <= 0)
            {
                percentageOfBar = 0;
                return String.Empty;
            }

            double percentageOfBeatNote = deltaTicks / division;
            percentageOfBar = (1.0 / beatsPerBar) * percentageOfBeatNote;

            for (int noteLength = 32; noteLength >= 1; noteLength -= 1)
            {
                double absoluteNoteLength = (1.0 / noteLength);

                if (percentageOfBar <= absoluteNoteLength)
                {
                    if (noteLength < 2)
                        noteLength = 2;

                    int subtractDuration;

                    if (noteLength == 32)
                        subtractDuration = 32;
                    else if (noteLength >= 16)
                        subtractDuration = 16;
                    else if (noteLength >= 8)
                        subtractDuration = 8;
                    else if (noteLength >= 4)
                        subtractDuration = 4;
                    else
                        subtractDuration = 2;

                    if (noteLength >= 17)
                        duration = 32;
                    else if (noteLength >= 9)
                        duration = 16;
                    else if (noteLength >= 5)
                        duration = 8;
                    else if (noteLength >= 3)
                        duration = 4;
                    else
                        duration = 2;

                    double currentTime = 0;

                    while (currentTime < (noteLength - subtractDuration))
                    {
                        var addtime = 1 / ((subtractDuration / beatNote) * Math.Pow(2, dots));
                        if (addtime <= 0) break;
                        currentTime += addtime;
                        if (currentTime <= (noteLength - subtractDuration))
                        {
                            dots++;
                        }
                        if (dots >= 4) break;
                    }

                    break;
                }
            }

            return duration + new String('.', dots);
        }
    }
}
