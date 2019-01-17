using DPA_Musicsheets.Commands.Export;
using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Chain
{
    class SaveAsLilypondHandler : AbstractShortcutHandler
    {
        private LilypondExportCommand liypondExportCommand = new LilypondExportCommand();

        public override object Handle(List<System.Windows.Input.Key> keyDown, BlockContainer _container)
        {
            if (
                keyDown.Contains(System.Windows.Input.Key.LeftCtrl)
                && keyDown.Contains(System.Windows.Input.Key.S)
                )
            {
                System.Console.WriteLine("Saving as Lilypond.");
                liypondExportCommand.Execute(_container);
                return "";
            }
            else
            {
                return base.Handle(keyDown, _container);
            }
        }
    }
}
