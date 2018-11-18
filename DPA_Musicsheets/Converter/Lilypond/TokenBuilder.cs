using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Lilypond
{
    interface ITokenBuilder
    {
        Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken);
    }

    abstract class TokenBuilder : ITokenBuilder
    {
        protected TokenBuilderFactory builderFactory;

        public TokenBuilder(TokenBuilderFactory builderFactory) => this.builderFactory = builderFactory;

        public abstract Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken);
        protected Block CreateBlock(LinkedListNode<LilypondToken> currentToken)
        {
            var block = new Block();
            currentToken = currentToken.Next;
                
            while (currentToken != null && currentToken.Value.TokenKind != LilypondTokenKind.SectionEnd)
            {
                var token = builderFactory.GetBuilder(currentToken.Value.TokenKind).BuildToken(block, currentToken);
                block.Add(token);

                currentToken = currentToken.Next;
            }

            return block;
        }
    }

    class BlockBuilder : TokenBuilder
    {
        public BlockBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            return CreateBlock(currentToken);
        }
    }

    class ClefBuilder : TokenBuilder
    {
        public ClefBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            currentToken = currentToken.Next;
            switch (currentToken.Value.Value)
            {
                case "treble":
                case "g":
                    return new Clef(ClefType.treble, 2);
                case "alto":
                case "c":
                    return new Clef(ClefType.c, 3);
                case "bass":
                case "f":
                    return new Clef(ClefType.f, 4);
                default:
                    return new NullToken();
            }
        }
    }

    class TimeBuilder : TokenBuilder
    {
        public TimeBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            currentToken = currentToken.Next;
            string[] timeData = currentToken.Value.Value.Split('/');
            return new TimeSignature(int.Parse(timeData[0]), int.Parse(timeData[1]));
        }
    }

    class TempoBuilder : TokenBuilder
    {
        public TempoBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            currentToken = currentToken.Next;
            string[] tempoData = currentToken.Value.Value.Split('=');
            return new Tempo(int.Parse(tempoData[0]), int.Parse(tempoData[1]));
        }
    }

    class RepeatBuilder : TokenBuilder
    {
        public RepeatBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            currentToken = currentToken.Next;
            if (currentToken.Value.Value != "volta")
            {
                throw new Exception("Invalid lilypond file.");
            }

            currentToken = currentToken.Next;
            int repeatAmount = int.Parse(currentToken.Value.Value);
            currentToken = currentToken.Next;

            if (currentToken.Value.Value != "{")
            {
                throw new Exception("Invalid lilypond file at repeat.");
            }

            Block nBlock = CreateBlock(currentToken);
            var repeated = new Repeat(nBlock, repeatAmount);
            
            return repeated;
        }
    }

    class AlternativeBuilder : TokenBuilder
    {
        public AlternativeBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            currentToken = currentToken.Next;
            if (currentToken.Value.Value != "{")
            {
                throw new Exception("Invalid Lilypond file.");
            }

            if (block.Last() is Repeat repeat)
            {
                repeat.toRepeat = CreateBlock(currentToken);
            }
            else
            {
                throw new Exception("Invalid Lilypond file.");
            }

            return new NullToken();
        }
    }

    class NoteBuilder : TokenBuilder
    {
        public NoteBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            var newNote = new Note();
            newNote.length = HandleLength(currentToken.Value.Value);
            newNote.key = HandleKey(currentToken.Value.Value);
            newNote.pitch = HandlePitch(currentToken.Value.Value);
            newNote.octave = HandleOctave(newNote.pitch, currentToken.Value.Value);

            return newNote;
        }

        private int HandleOctave(Pitch pitch, string value)
        {
            int val = Regex.Matches(value, ",").Count;
            if (val > 0)
            {
                return 1;
            }

            val = Regex.Matches(value, "'").Count;
            if (val > 0)
            {
                return -1;
            }

            return 0;
        }

        private string HandleLength(string value)
        {
            // Retreive the base lenght of the note.
            var length = float.Parse(Regex.Match(value, @"[0-9]+").Value);
            // Retreive the amount of dots used in the note.
            var dotAmount = value.Count(i => i.Equals('.'));

            return "" + (1 / length) * (2 - 1 / Math.Pow(2, dotAmount));
        }

        private Key HandleKey(string value)
        {
            var val = Regex.Matches(value, "is").Count - Regex.Matches(value, "es").Count;
            KeyType key;

            if (val < 0)
            {
                key = KeyType.FLAT;
            }
            else if (val > 0)
            {
                key = KeyType.SHARP;
            }
            else
            {
                key = KeyType.NORM;
            }

            return new Key(key, 0);
        }

        private Pitch HandlePitch(string value)
        {
            int index;
            if (value[0] == '~')
            {
                index = 1;
            } else
            {
                index = 0;
            }
            return (Pitch)Enum.Parse(typeof(Pitch), value[index].ToString().ToUpper());
        }
    }

    class BarBuilder : TokenBuilder
    {
        public BarBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            return new Bar();
        }
    }

    class NullBuilder : TokenBuilder
    {
        public NullBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            return new NullToken();
        }
    }

    class SectionStartBuilder : TokenBuilder
    {
        public SectionStartBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            return CreateBlock(currentToken);
        }
    }

    class SectionEndBuilder : TokenBuilder
    {
        public SectionEndBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            return block;
        }
    }

    class RestBuilder : TokenBuilder
    {
        public RestBuilder(TokenBuilderFactory builderFactory) : base(builderFactory)
        {
        }

        public override Token BuildToken(Block block, LinkedListNode<LilypondToken> currentToken)
        {
            return new Rest(HandleLength(currentToken.Value.Value));
        }

        private string HandleLength(string value)
        {
            // Retreive the base lenght of the note.
            var length = float.Parse(Regex.Match(value, @"[0-9]+").Value);
            // Retreive the amount of dots used in the note.
            var dotAmount = value.Count(i => i.Equals('.'));

            return "" + (1 / length) * (2 - 1 / Math.Pow(2, dotAmount));
        }
    }

    class TokenBuilderFactory
    {
        private Dictionary<LilypondTokenKind, TokenBuilder> builders;

        public TokenBuilderFactory()
        {
            builders = new Dictionary<LilypondTokenKind, TokenBuilder>();
            builders.Add(LilypondTokenKind.Clef, new ClefBuilder(this));
            builders.Add(LilypondTokenKind.Time, new TimeBuilder(this));
            builders.Add(LilypondTokenKind.Tempo, new TempoBuilder(this));
            // builders.Add(LilypondTokenKind.Extend, new ExtendBuilder());
            builders.Add(LilypondTokenKind.Bar, new BarBuilder(this));
            builders.Add(LilypondTokenKind.Repeat, new RepeatBuilder(this));
            builders.Add(LilypondTokenKind.Alternative, new AlternativeBuilder(this));
            builders.Add(LilypondTokenKind.Note, new NoteBuilder(this));
            builders.Add(LilypondTokenKind.SectionStart, new SectionStartBuilder(this));
            builders.Add(LilypondTokenKind.SectionEnd, new SectionEndBuilder(this));
            builders.Add(LilypondTokenKind.Rest, new RestBuilder(this));
            builders.Add(LilypondTokenKind.Unknown, new NullBuilder(this));
        }

        public ITokenBuilder GetBuilder(LilypondTokenKind token)
        {
            if (builders.ContainsKey(token))
            {
                return builders[token];
            }

            return builders[LilypondTokenKind.Unknown];
        }
    }
}
