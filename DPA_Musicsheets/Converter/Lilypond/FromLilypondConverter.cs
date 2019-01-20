using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter.Token;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.Converter.Lilypond
{
    public class FromLilypondConverter : IFromConverter
    {
        private LinkedList<LilypondToken> tokens;
        private readonly ToTokenConverter toTokenConverter;

        public FromLilypondConverter()
        {
            toTokenConverter = new ToTokenConverter();
        }

        public Block ConvertTo(string text)
        {
            tokens = toTokenConverter.GetTokensFromLilypond(text);
            toTokenConverter.DebugTokens(tokens);

            var director = new BlockDirector();
            var block = director.Build(tokens);

            return (Block)block;
        }

        public Block ConvertToFromFile(string fileName)
        {
            var content = GetContent(fileName);

            return ConvertTo(content);
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
    }
}
