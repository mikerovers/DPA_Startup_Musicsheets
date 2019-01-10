using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Lilypond
{
    public class ToLilypondConverter : IToConverter<string>
    {
        private StringBuilder _stringBuilder;

        public ToLilypondConverter()
        {
            _stringBuilder = new StringBuilder();
        }

        public string ConvertTo(Block block)
        {
            _stringBuilder.Clear();
            _stringBuilder.AppendLine(@"\relative c' {");
            var lilyVisitor = new ToLilyVisitor(_stringBuilder);
            lilyVisitor.Visit(block);
            _stringBuilder.Append("}");

            return _stringBuilder.ToString();
        }
    }
}
