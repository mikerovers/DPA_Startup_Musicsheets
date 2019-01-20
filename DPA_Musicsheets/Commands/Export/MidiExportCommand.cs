using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter.Midi;
using DPA_Musicsheets.Managers;
using Microsoft.Win32;

namespace DPA_Musicsheets.Commands.Export
{
    class MidiExportCommand : IShortcutCommand
    {
        public string pattern => "export_midi";
        private ToMidiConverter toMidiConverter = new ToMidiConverter();
        public string FileName = null;

        public void Execute(BlockContainer container)
        {
            if (FileName == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Midi|*.mid" };

                if (saveFileDialog.ShowDialog() == true)
                {
                    FileName = saveFileDialog.FileName;
                }
                else { return; }
            }

            var sequence = toMidiConverter.ConvertTo(container.Block);
            sequence.Save(FileName);
        }
    }
}
