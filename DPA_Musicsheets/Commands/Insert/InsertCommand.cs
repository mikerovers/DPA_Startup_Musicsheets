using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Managers;

namespace DPA_Musicsheets.Commands
{
    public class InsertCommand : IShortcutCommand
    {
        public string pattern => "insert_text";
        private FromLilypondConverter fromLilypondConverter = new FromLilypondConverter();
        private ToLilypondConverter toLilypondConverter = new ToLilypondConverter();
        private string _text;

        public InsertCommand(string text)
        {
            _text = text;
        }

        public void Execute(BlockContainer container)
        {
            var text = toLilypondConverter.ConvertTo(container.Block);
            text = text.Insert(container.CarotIndex, _text);
            // text = text.Substring(0, container.CarotIndex) + _text + text.Substring(container.CarotIndex, text.Length - container.CarotIndex);
            container.Block = fromLilypondConverter.ConvertTo(text);
        }
    }
}
