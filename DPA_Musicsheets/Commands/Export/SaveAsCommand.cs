using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DPA_Musicsheets.Managers;
using Microsoft.Win32;

namespace DPA_Musicsheets.Commands.Export
{
    class SaveAsCommand : IShortcutCommand
    {
        public string pattern => throw new NotImplementedException();

        public void Execute(BlockContainer container)
        {
            IShortcutCommand exportCommand = new NullCommand();
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Lilypond|*.ly|Midi|*.mid|PDF|*.pdf" };
            if (saveFileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(saveFileDialog.FileName);
                if (extension.EndsWith(".mid"))
                {
                    // TODO
                }
                else if (extension.EndsWith(".ly"))
                {
                    exportCommand = new LilypondExportCommand();
                    ((LilypondExportCommand)exportCommand).FileName = saveFileDialog.FileName;
                }
                else if (extension.EndsWith(".pdf"))
                {
                    exportCommand = new PDFExportCommand();
                    ((PDFExportCommand)exportCommand).FileName = saveFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show($"Extension {extension} is not supported.");
                }

                exportCommand.Execute(container);
            }
        }
    }
}
