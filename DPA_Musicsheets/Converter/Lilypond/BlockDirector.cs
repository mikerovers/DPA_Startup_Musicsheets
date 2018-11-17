using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Lilypond
{
    class BlockDirector
    {
        private Note NoteToExtend;
        private LinkedListNode<LilypondToken> currentToken;

        public Block Build(LinkedList<LilypondToken> tokens)
        {
            currentToken = tokens.First;
            NoteToExtend = null;

            return ConstructBlock();
        }

        private Block ConstructBlock()
        {
            Block block = new Block();
            var tokenFactory = new TokenBuilderFactory();
            var builder = new BlockBuilder(tokenFactory);

            return (Block)builder.BuildToken(block, currentToken);

            currentToken = currentToken.Next;
                        
            while (currentToken != null)
            {
                switch (currentToken.Value.TokenKind)
                {
                    case LilypondTokenKind.SectionStart:
                        block.Add(ConstructBlock());

                        break;
                    case LilypondTokenKind.SectionEnd:
                        return block;
                    case LilypondTokenKind.Clef:
                        currentToken = currentToken.Next;
                        switch (currentToken.Value.Value)
                        {
                            case "treble":
                            case "g":
                                block.Add(new Clef(ClefType.g, 2));

                                break;
                            case "alto":
                            case "c":
                                block.Add(new Clef(ClefType.c, 3));

                                break;
                            case "bass":
                            case "f":
                                block.Add(new Clef(ClefType.f, 4));

                                break;
                        }

                        break;
                    case LilypondTokenKind.Time:
                        currentToken = currentToken.Next;
                        string[] timeData = currentToken.Value.Value.Split('/');
                        block.Add(new TimeSignature(int.Parse(timeData[0]), int.Parse(timeData[1])));

                        break;
                    case LilypondTokenKind.Tempo:
                        currentToken = currentToken.Next;
                        string[] tempoData = currentToken.Value.Value.Split('=');
                        block.Add(new Tempo(int.Parse(tempoData[0]), int.Parse(tempoData[1])));

                        break;
                    case LilypondTokenKind.Extend:
                        if (block.Last() is Note note)
                        {
                            NoteToExtend = note;
                        }

                        break;
                    case LilypondTokenKind.Bar:
                        block.Add(new Bar());

                        break;
                    case LilypondTokenKind.Repeat:
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

                        Block nBlock = ConstructBlock();
                        var repeated = new Repeat(nBlock, repeatAmount);
                        block.Add(repeated);

                        break;
                    case LilypondTokenKind.Alternative:
                        currentToken = currentToken.Next;
                        if (currentToken.Value.Value != "{")
                        {
                            throw new Exception("Invalid Lilypond file.");
                        }

                        var b2 = block;

                        if (block.Last() is Repeat repeat)
                        {
                            repeat.toRepeat = ConstructBlock();
                        }
                        else
                        {
                            throw new Exception("Invalid Lilypond file.");
                        }

                        break;
                    case LilypondTokenKind.Note:

                        var newNote = new Note();
                        newNote.length = HandleLength(currentToken.Value.Value);
                        newNote.key = HandleKey(currentToken.Value.Value);
                        newNote.pitch = HandlePitch(currentToken.Value.Value);
                        block.Add(newNote);

                        break;
                }

                if (currentToken != null && currentToken.Next != null)
                {
                    currentToken = currentToken.Next;
                } else
                {
                    currentToken = null;
                }
            }

            return block;
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
            } else if (val > 0)
            {
                key = KeyType.SHARP;
            } else
            {
                key = KeyType.NORM;
            }

            return new Key(key, 0);
        }

        private Pitch HandlePitch(string value)
        {
            // Pitch is in first character.
            return (Pitch)Enum.Parse(typeof(Pitch), value[0].ToString().ToUpper());
        }
    }
}
