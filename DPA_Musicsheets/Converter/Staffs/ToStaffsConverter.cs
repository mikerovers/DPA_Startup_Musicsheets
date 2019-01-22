using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Converter.Token;
using DPA_Musicsheets.Models;
using PSAMControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DPA_Musicsheets.Converter.Staffs
{
    class ToStaffsConverter : IToConverter<IList<MusicalSymbol>>
    {
        private static List<Char> notesorder = new List<Char> { 'c', 'd', 'e', 'f', 'g', 'a', 'b' };
        PSAMControlLibrary.Clef currentClef = null;
        int previousOctave = 4;
        char previousNote = 'c';
        bool inRepeat = false;
        bool inAlternative = false;
        int alternativeRepeatNumber = 0;
        LilypondToken currentToken = null;

        public IList<MusicalSymbol> ConvertTo(Block block)
        {
            var toLilypondConvereter = new ToLilypondConverter();
            var toTokensConverter = new ToTokenConverter();

            return ConvertTo(toTokensConverter.GetTokensFromLilypond(toLilypondConvereter.ConvertTo(block)));
        }

        public IList<MusicalSymbol> ConvertTo(LinkedList<LilypondToken> tokens)
        {
            List<MusicalSymbol> symbols = new List<MusicalSymbol>();
            var tokenDecoratorFactory = new TokenDecoratorFactory(this);
            TokenDecorator currentDecorator = null;

            currentToken = tokens.First();
            while (currentToken != null)
            {
                var newDecorator = tokenDecoratorFactory.GetDecorator(currentToken);
                currentDecorator = newDecorator;
                currentToken = currentToken.NextToken;
            }

            currentDecorator.Perform(symbols);

            return symbols;
        }

        internal interface ITokenDecorator
        {
            void Perform(IList<MusicalSymbol> symbols);
        }

        internal abstract class TokenDecorator : ITokenDecorator
        {
            private ITokenDecorator decorator;
            protected ToStaffsConverter parent;
            protected LilypondToken currentToken;

            public TokenDecorator(ToStaffsConverter parent, ITokenDecorator decorator)
            {
                this.parent = parent;
                this.decorator = decorator;
                this.currentToken = parent.currentToken;
            }

            public void SetDecorator(ITokenDecorator decorator)
            {
                this.decorator = decorator;
            }

            virtual public void Perform(IList<MusicalSymbol> symbols)
            {
                if (decorator != null)
                    decorator.Perform(symbols);
            }
        }

        internal class RepeatDecorator : TokenDecorator
        {
            public RepeatDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {}

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);

                parent.inRepeat = true;
                symbols.Add(new Barline() { RepeatSign = RepeatSignType.Forward });
            }
        }

        internal class SectionEndDecorator : TokenDecorator
        {
            public SectionEndDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {
            }

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);

                if (parent.inRepeat && currentToken.NextToken?.TokenKind != LilypondTokenKind.Alternative)
                {
                    parent.inRepeat = false;
                    symbols.Add(new Barline() { RepeatSign = RepeatSignType.Backward, AlternateRepeatGroup = parent.alternativeRepeatNumber });
                }
                else if (parent.inAlternative && parent.alternativeRepeatNumber == 1)
                {
                    parent.alternativeRepeatNumber++;
                    symbols.Add(new Barline() { RepeatSign = RepeatSignType.Backward, AlternateRepeatGroup = parent.alternativeRepeatNumber });
                }
                else if (parent.inAlternative && currentToken.NextToken.TokenKind == LilypondTokenKind.SectionEnd)
                {
                    parent.inAlternative = false;
                    parent.alternativeRepeatNumber = 0;
                }
            }
        }

        internal class SectionStartDecorator : TokenDecorator
        {
            public SectionStartDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {
            }

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);

                if (parent.inAlternative && currentToken.PreviousToken.TokenKind != LilypondTokenKind.SectionEnd)
                {
                    parent.alternativeRepeatNumber++;
                    symbols.Add(new Barline() { AlternateRepeatGroup = parent.alternativeRepeatNumber });
                }
            }
        }

        internal class AlternativeDecorator : TokenDecorator
        {
            public AlternativeDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {
                parent.currentToken = parent.currentToken.NextToken;
            }

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);

                parent.inAlternative = true;
                parent.inRepeat = false;
            }
        }

        internal class NoteDecorator : TokenDecorator
        {
            private LilypondToken currentToken;

            public NoteDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {
                currentToken = parent.currentToken;
            }

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);

                // Tied
                // TODO: A tie, like a dot and cross or mole are decorations on notes. Is the DECORATOR pattern of use here?
                NoteTieType tie = NoteTieType.None;
                if (currentToken.Value.StartsWith("~"))
                {
                    tie = NoteTieType.Stop;
                    var lastNote = symbols.Last(s => s is PSAMControlLibrary.Note) as PSAMControlLibrary.Note;
                    if (lastNote != null) lastNote.TieType = NoteTieType.Start;
                    currentToken.Value = currentToken.Value.Substring(1);
                }
                // Length
                int noteLength = Int32.Parse(Regex.Match(currentToken.Value, @"\d+").Value);
                // Crosses and Moles
                int alter = 0;
                alter += Regex.Matches(currentToken.Value, "is").Count;
                alter -= Regex.Matches(currentToken.Value, "es|as").Count;
                // Octaves
                int distanceWithPreviousNote = notesorder.IndexOf(currentToken.Value[0]) - notesorder.IndexOf(parent.previousNote);
                if (distanceWithPreviousNote > 3) // Shorter path possible the other way around
                {
                    distanceWithPreviousNote -= 7; // The number of notes in an octave
                }
                else if (distanceWithPreviousNote < -3)
                {
                    distanceWithPreviousNote += 7; // The number of notes in an octave
                }

                if (distanceWithPreviousNote + notesorder.IndexOf(parent.previousNote) >= 7)
                {
                    parent.previousOctave++;
                }
                else if (distanceWithPreviousNote + notesorder.IndexOf(parent.previousNote) < 0)
                {
                    parent.previousOctave--;
                }

                // Force up or down.
                parent.previousOctave += currentToken.Value.Count(c => c == '\'');
                parent.previousOctave -= currentToken.Value.Count(c => c == ',');

                parent.previousNote = currentToken.Value[0];

                var note = new PSAMControlLibrary.Note(currentToken.Value[0].ToString().ToUpper(), alter, parent.previousOctave, (MusicalSymbolDuration)noteLength, NoteStemDirection.Up, tie, new List<NoteBeamType>() { NoteBeamType.Single });
                note.NumberOfDots += currentToken.Value.Count(c => c.Equals('.'));

                symbols.Add(note);
            }
        }

        internal class RestDecorator : TokenDecorator
        {
            public RestDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {
            }

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);

                var restLength = Int32.Parse(currentToken.Value[1].ToString());
                symbols.Add(new PSAMControlLibrary.Rest((MusicalSymbolDuration)restLength));
            }
        }

        internal class BarDecorator : TokenDecorator
        {
            public BarDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {
            }

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);

                symbols.Add(new Barline() { AlternateRepeatGroup = parent.alternativeRepeatNumber });
            }
        }

        internal class ClefDecorator : TokenDecorator
        {
            private LilypondToken trebleToken;

            public ClefDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {
                parent.currentToken = parent.currentToken.NextToken;
                trebleToken = parent.currentToken;
            }

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);

                if (trebleToken.Value == "treble")
                    parent.currentClef = new PSAMControlLibrary.Clef(PSAMControlLibrary.ClefType.GClef, 2);
                else if (parent.currentToken.Value == "bass")
                    parent.currentClef = new PSAMControlLibrary.Clef(PSAMControlLibrary.ClefType.FClef, 4);
                else
                    throw new NotSupportedException($"Clef {trebleToken.Value} is not supported.");

                symbols.Add(parent.currentClef);
            }
        }

        internal class TimeDecorator : TokenDecorator
        {
            private LilypondToken timeToken;

            public TimeDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {
                parent.currentToken = parent.currentToken.NextToken;
                timeToken = parent.currentToken;
            }

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);

                var times = timeToken.Value.Split('/');
                symbols.Add(new PSAMControlLibrary.TimeSignature(TimeSignatureType.Numbers, UInt32.Parse(times[0]), UInt32.Parse(times[1])));
            }
        }

        internal class NullDecorator : TokenDecorator
        {
            public NullDecorator(ToStaffsConverter parent, ITokenDecorator decorator) : base(parent, decorator)
            {
            }

            public override void Perform(IList<MusicalSymbol> symbols)
            {
                base.Perform(symbols);
            }
        }

        internal class TokenDecoratorFactory
        {
            TokenDecorator currentDecorator = null;
            ToStaffsConverter toStaffsConverter;

            public TokenDecoratorFactory(ToStaffsConverter toStaffsConverter)
            {
                this.toStaffsConverter = toStaffsConverter;
            }

            public TokenDecorator GetDecorator(LilypondToken token)
            {
                switch (token.TokenKind)
                {
                    case LilypondTokenKind.Unknown:
                        currentDecorator = new NullDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.Repeat:
                        currentDecorator = new RepeatDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.SectionEnd:
                        currentDecorator = new SectionEndDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.SectionStart:
                        currentDecorator = new SectionStartDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.Alternative:
                        currentDecorator = new AlternativeDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.Note:
                        currentDecorator = new NoteDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.Rest:
                        currentDecorator = new RestDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.Bar:
                        currentDecorator = new BarDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.Clef:
                        currentDecorator = new ClefDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.Time:
                        currentDecorator = new TimeDecorator(toStaffsConverter, currentDecorator);
                        break;
                    case LilypondTokenKind.Tempo:
                        currentDecorator = new NullDecorator(toStaffsConverter, currentDecorator);
                        break;
                    default:
                        currentDecorator = new NullDecorator(toStaffsConverter, currentDecorator);
                        break;
                }

                return currentDecorator;
            }
        }
    }
}
