using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Lilypond
{
    class BlockBuilder
    {
        private Note NoteToExtend;
        private LinkedListNode<LilypondToken> currentToken;

        public Block Build(LinkedList<LilypondToken> tokens)
        {
            currentToken = tokens.First;
            NoteToExtend = null;

            return newBlock();
        }

        private Block newBlock()
        {
            var block = new Block();
            currentToken = currentToken.Next;

            while (currentToken != null)
            {
                switch (currentToken.Value.TokenKind)
                {
                    case LilypondTokenKind.SectionStart:
                        block.Add(newBlock());

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

                        Block nBlock = newBlock();
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
                            repeat.toRepeat = newBlock();
                        }
                        else
                        {
                            throw new Exception("Invalid Lilypond file.");
                        }

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
    }
}
