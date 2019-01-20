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
            var content = lilyContent.Trim().ToLower().Replace("\r\n", " ").Replace("\n", " ").Replace("  ", " ").Replace("\t", "");
            var c1 = Regex.Replace(content, " {2,}", " ");

            foreach (string s in content.Split(' ').Where(item => item.Length > 0))
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
        }

        public void DebugTokens(LinkedList<LilypondToken> tokens)
        {
            foreach (var token in tokens)
            {
                System.Console.WriteLine($"{token.TokenKind} - {token.Value}");
            }
        }
    }
}
