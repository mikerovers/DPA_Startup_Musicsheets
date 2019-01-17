using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Managers;
using Microsoft.Win32;

namespace DPA_Musicsheets.Commands.Open
{
    class OpenFileCommand : IShortcutCommand
    {
        public string pattern => "open";

        public void Execute(BlockContainer container)
        {
            var openFileDialog = new OpenFileDialog() { Filter = "Midi or LilyPond files (*.mid *.ly)|*.mid;*.ly" };
            if (openFileDialog.ShowDialog() == true)
            {
                container.OpenFile(openFileDialog.FileName);
            }
        }
    }
}
