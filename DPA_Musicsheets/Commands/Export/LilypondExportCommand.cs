using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Managers;
using DPA_Musicsheets.State;
using Microsoft.Win32;

namespace DPA_Musicsheets.Commands.Export
{
    class LilypondExportCommand : IShortcutCommand
    {
        public string pattern => "export_lilypond";
        private ToLilypondConverter toLilypondConverter = new ToLilypondConverter();
        public string FileName = null;

        public void Execute(BlockContainer container)
        {
            if (FileName == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Lilypond|*.ly" };

                if (saveFileDialog.ShowDialog() == true)
                {
                    FileName = saveFileDialog.FileName;
                } else { return; }
            }

            var lilypondText = toLilypondConverter.ConvertTo(container.Block);
            using (StreamWriter outputFile = new StreamWriter(FileName))
            {
                outputFile.Write(lilypondText);
                outputFile.Close();
            }

            container.textEditState = new TextNotEditedState(container);
        }
    }
}
