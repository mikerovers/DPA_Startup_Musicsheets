using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Lilypond
{
    public class BlockDirector
    {
        private Note NoteToExtend;
        private LinkedListNode<LilypondToken> currentToken;
        private LinkedList<LilypondToken> tokens;

        public Block Build(LinkedList<LilypondToken> tokens)
        {
            currentToken = tokens.First;
            NoteToExtend = null;
            this.tokens = tokens;

            return ConstructBlock();
        }

        private Block ConstructBlock()
        {
            Block block = new Block();
            var tokenFactory = new TokenBuilderFactory();
            var builder = new BlockBuilder(tokenFactory);

            Queue<LilypondToken> queue = new Queue<LilypondToken>(tokens);

            return (Block)builder.BuildToken(block, queue); 
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
