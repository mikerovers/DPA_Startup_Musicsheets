using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Managers;

namespace DPA_Musicsheets.Commands
{
    public class InsertTimeCommand : IShortcutCommand
    {
        public string pattern => "insert_time";
        private FromLilypondConverter fromLilypondConverter = new FromLilypondConverter();
        private ToLilypondConverter toLilypondConverter = new ToLilypondConverter();

        public void Execute(BlockContainer container)
        {
            var text = toLilypondConverter.ConvertTo(container.Block);
            text = text.Substring(0, container.CarotIndex) + "\time 4/4" + text.Substring(container.CarotIndex, text.Length - container.CarotIndex);
            container.Block = fromLilypondConverter.ConvertTo(text);
        }
    }
}
