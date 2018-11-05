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
    class FromMidiConverter
    {
        private Block block;
        public int previousMidiKey = 60;
        public bool startedNoteIsClosed = true;
        public int previousNoteAbsoluteTicks = 0;
        public double percentageOfBarReached = 0;
        public int division;

        public int _beatNote = 4;    // De waarde van een beatnote.
        public int _bpm = 120;       // Aantal beatnotes per minute.
        public int _beatsPerBar;     // Aantal beatnotes per maat.


        public FromMidiConverter()
        {
            block = new Block();
        }

        public Block convert(Sequence sequence)
        {
            block = new Block();
            division = sequence.Division;

            for (int i = 0; i < sequence.Count(); i++)
            {
                Track track = sequence[i];

                foreach (MidiEvent midiEvent in track.Iterator())
                {
                    IMidiMessage midiMessage = midiEvent.MidiMessage;
                    switch (midiMessage.MessageType)
                    {
                        case MessageType.Meta:
                            ConvertMetaMessage((MetaMessage) midiMessage, midiEvent);

                            break;
                        case MessageType.Channel:
                            ConvertChannelMessage((ChannelMessage) midiMessage, midiEvent);

                            break;
                    }
                }
            }

            return block;
        }

        public void ConvertMetaMessage(MetaMessage message, MidiEvent midiEvent)
        {
            switch (message.MetaType)
            {
                case MetaType.TimeSignature:
                    HandleTimeSignature(message, midiEvent);

                    break;
                case MetaType.Tempo:
                    HandleTempo(message, midiEvent);

                    break;  
            }
        }

        public void ConvertChannelMessage(ChannelMessage message, MidiEvent midiEvent)
        {
            if (message.Data2 > 0)
            {
                HandleNoteOn(message, midiEvent);
            }
            else if (!startedNoteIsClosed)
            {
                HandleNoteOff(message, midiEvent);
            }
        }

        public TimeSignature HandleTimeSignature(MetaMessage metaMessage, MidiEvent e)
        {
            byte[] timeSignatureBytes = metaMessage.GetBytes();
            _beatNote = timeSignatureBytes[0];
            _beatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));

            var signature = new Models.TimeSignature(_beatNote, _beatsPerBar);
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
            double percentageOfBar;
            string length = MidiToLilyHelper.GetLilypondNoteLength(previousNoteAbsoluteTicks, e.AbsoluteTicks, division, _beatNote, _beatsPerBar, out percentageOfBar);
            CurNote.length = length;

            previousNoteAbsoluteTicks = e.AbsoluteTicks;
            percentageOfBarReached += percentageOfBar;
            startedNoteIsClosed = true;

            block.Add(CurNote);
        }

        public Note CurNote;

        public void HandleNoteOn(ChannelMessage channelMessage, MidiEvent e)
        {
            CurNote = new Note();
            CurNote.key = MidiToLilyHelper.GetLilyNoteName(previousMidiKey, channelMessage.Data1);
            previousMidiKey = channelMessage.Data1;
            startedNoteIsClosed = false;
        }

        public void HandleNoteOff(ChannelMessage channelMessage, MidiEvent e)
        {
            double percentageOfBar;
            string length = MidiToLilyHelper.GetLilypondNoteLength(previousNoteAbsoluteTicks, e.AbsoluteTicks, division, _beatNote, _beatsPerBar, out percentageOfBar);
            CurNote.length = length;

            previousNoteAbsoluteTicks = e.AbsoluteTicks;
            percentageOfBarReached += percentageOfBar;
            startedNoteIsClosed = true;

            block.Add(CurNote);
        }
    
    }
}
