using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Lilypond
{
    class ToLilypondConverter : IToConverter<string>
    {
        private StringBuilder stringBuilder;

        public ToLilypondConverter()
        {
            stringBuilder = new StringBuilder();
        }

        public string ConvertTo(Block block)
        {
            stringBuilder.AppendLine(@"\relative c' {");
            var lilyVisitor = new ToLilyVisitor(stringBuilder);
            lilyVisitor.Visit(block);

            return stringBuilder.ToString();
        }
    }
}
