using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.Converter.Lilypond
{
    class FromLilypondConverter : IFromConverter
    {
        private LinkedList<LilypondToken> tokens;

        public Block ConvertTo(string fileName)
        {
            var content = GetContent(fileName);
            tokens = GetTokensFromLilypond(content);
            DebugTokens(tokens);

            var builder = new BlockBuilder();
            var block = builder.Build(tokens);
            var toli = new ToLilypondConverter();
            // var lil = toli.ConvertTo(block);

            return builder.Build(tokens);
        }

        private string GetContent(string fileName)
        {
            StringBuilder builder = new StringBuilder();

            foreach(var line in File.ReadAllLines(fileName))
            {
                builder.Append(line);
            }

            return builder.ToString();
        }

        private LinkedList<LilypondToken> GetTokensFromLilypond(string lilyContent)
        {
            // Make sure that 2 spaces or more do not exist.
            var blankRegex = new Regex(@"\s+");
            lilyContent = blankRegex.Replace(lilyContent, " ");
            var tokens = new LinkedList<LilypondToken>();
            var lines = lilyContent.Split(' ').ToList<string>();
            var tokenFactory = new TokenKindFactory();
            
            foreach(var line in lines)
            {
                var token = new LilypondToken();
                token.Value = line;
                token.TokenKind = tokenFactory.GetToken(line);

                if (token.TokenKind == LilypondTokenKind.Unknown)
                {
                    token.TokenKind = ResolveUnknownTokenKind(line);
                }

                tokens.AddLast(token);
            }

            return tokens;
        }
        
        private void DebugTokens(LinkedList<LilypondToken> tokens)
        {
            foreach(var token in tokens)
            {
                System.Console.WriteLine($"{token.TokenKind} - {token.Value}");
            }
        }

        private LilypondTokenKind ResolveUnknownTokenKind(string line)
        {
            // Check if the line is a note.
            if (new Regex(@"[a-g][,'eis]*[0-9]+[.]*").IsMatch(line))
            {
                return LilypondTokenKind.Note;
            }
            // Check if the line is a rest.
            else if (new Regex(@"r.*?[0-9][.]*").IsMatch(line))
            {
                return LilypondTokenKind.Rest;
            }

            return LilypondTokenKind.Unknown;
        }

        internal class TokenKindFactory
        {
            public LilypondTokenKind GetToken(string line)
            {
                switch (line)
                {
                    case @"\alternative":
                        return LilypondTokenKind.Alternative;
                    case @"\repeat":
                        return LilypondTokenKind.Repeat;
                    case @"\time":
                        return LilypondTokenKind.Time;
                    case @"\tempo":
                        return LilypondTokenKind.Tempo;
                    case @"\relative":
                        return LilypondTokenKind.Staff;
                    case @"\clef":
                        return LilypondTokenKind.Clef;
                    case @"|":
                        return LilypondTokenKind.Bar;
                    case @"~":
                        return LilypondTokenKind.Extend;
                    case @"{":
                        return LilypondTokenKind.SectionStart;
                    case @"}":
                        return LilypondTokenKind.SectionEnd;
                    default:
                        return LilypondTokenKind.Unknown;
                }
            }
        }
    }
}
