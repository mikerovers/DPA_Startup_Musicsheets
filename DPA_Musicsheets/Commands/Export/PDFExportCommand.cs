using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Managers;
using Microsoft.Win32;

namespace DPA_Musicsheets.Commands.Export
{
    class PDFExportCommand : IShortcutCommand
    {
        public string pattern => "export_pdf";
        private ToLilypondConverter toLilypondConverter = new ToLilypondConverter();
        private LilypondExportCommand lilypondExportCommand = new LilypondExportCommand();
        public string FileName = null;

        public void Execute(BlockContainer container)
        {
            if (FileName == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "PDF|*.pdf" };
                if (saveFileDialog.ShowDialog() == true)
                {
                    FileName = saveFileDialog.FileName;
                } else { return; }
            }

            string withoutExtension = Path.GetFileNameWithoutExtension(FileName);
            string tmpFileName = $"{FileName}-tmp.ly";
            lilypondExportCommand.FileName = tmpFileName;
            lilypondExportCommand.Execute(container);

            string lilypondLocation = @"C:\Program Files (x86)\LilyPond\usr\bin\lilypond.exe";
            string sourceFolder = Path.GetDirectoryName(tmpFileName);
            string sourceFileName = Path.GetFileNameWithoutExtension(tmpFileName);
            string targetFolder = Path.GetDirectoryName(FileName);
            string targetFileName = Path.GetFileNameWithoutExtension(FileName);

            var process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = sourceFolder,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("--pdf \"{0}\\{1}.ly\"", sourceFolder, sourceFileName),
                    FileName = lilypondLocation
                }
            };

            process.Start();
            while (!process.HasExited) { }
            if (sourceFolder != targetFolder || sourceFileName != targetFileName)
            {
                File.Move(sourceFolder + "\\" + sourceFileName + ".pdf", targetFolder + "\\" + targetFileName + ".pdf");
                File.Delete(tmpFileName);
            }
        }
    }
}
