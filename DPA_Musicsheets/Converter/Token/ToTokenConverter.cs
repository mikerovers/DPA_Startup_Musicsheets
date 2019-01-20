using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DPA_Musicsheets.Converter.Lilypond.FromLilypondConverter;

namespace DPA_Musicsheets.Converter.Token
{
    class ToTokenConverter
    {
        public LinkedList<LilypondToken> GetTokensFromLilypond(string lilyContent)
        {
            var tokens = new LinkedList<LilypondToken>();

            foreach (string s in lilyContent.Split(' ').Where(item => item.Length > 0))
            {
                LilypondToken token = new LilypondToken()
                {
                    Value = s
                };

                switch (s)
                {
                    case "\\relative": token.TokenKind = LilypondTokenKind.Staff; break;
                    case "\\clef": token.TokenKind = LilypondTokenKind.Clef; break;
                    case "\\time": token.TokenKind = LilypondTokenKind.Time; break;
                    case "\\tempo": token.TokenKind = LilypondTokenKind.Tempo; break;
                    case "\\repeat": token.TokenKind = LilypondTokenKind.Repeat; break;
                    case "\\alternative": token.TokenKind = LilypondTokenKind.Alternative; break;
                    case "{": token.TokenKind = LilypondTokenKind.SectionStart; break;
                    case "}": token.TokenKind = LilypondTokenKind.SectionEnd; break;
                    case "|": token.TokenKind = LilypondTokenKind.Bar; break;
                    default: token.TokenKind = LilypondTokenKind.Unknown; break;
                }

                if (token.TokenKind == LilypondTokenKind.Unknown && new Regex(@"[~]?[a-g][,'eis]*[0-9]+[.]*").IsMatch(s))
                {
                    token.TokenKind = LilypondTokenKind.Note;
                }
                else if (token.TokenKind == LilypondTokenKind.Unknown && new Regex(@"r.*?[0-9][.]*").IsMatch(s))
                {
                    token.TokenKind = LilypondTokenKind.Rest;
                }

                if (tokens.Last != null)
                {
                    tokens.Last.Value.NextToken = token;
                    token.PreviousToken = tokens.Last.Value;
                }

                tokens.AddLast(token);
            }

            return tokens;

            // Make sure that 2 spaces or more do not exist.
            var blankRegex = new Regex(@"\s+");
            lilyContent = blankRegex.Replace(lilyContent, " ");
            var tokenss = new LinkedList<LilypondToken>();
            var lines = lilyContent.Split(' ').ToList<string>();
            var tokenFactory = new LilypondTokenizer();

            foreach (var line in lines)
            {
                var token = new LilypondToken();
                token.Value = line;
                token.TokenKind = tokenFactory.GetToken(line);

                if (token.TokenKind == LilypondTokenKind.Unknown)
                {
                    token.TokenKind = ResolveUnknownTokenKind(line);
                }

                tokenss.AddLast(token);
            }

            return tokenss;
        }

        public void DebugTokens(LinkedList<LilypondToken> tokens)
        {
            foreach (var token in tokens)
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

        internal class LilypondTokenizer
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
