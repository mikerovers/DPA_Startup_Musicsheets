using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Chain
{
    class InsertClefHandler : AbstractShortcutHandler
    {
        private IShortcutCommand command;

        public override object Handle(List<System.Windows.Input.Key> keyDown, BlockContainer _container)
        {
            if (keyDown.Contains(System.Windows.Input.Key.LeftCtrl)
                && keyDown.Contains(System.Windows.Input.Key.E))
            {
                command = new InsertCommand("\\clef treble");
                command.Execute(_container);
                System.Console.WriteLine("Inputting clef.");

                return "";
            }
            else
            {
                return base.Handle(keyDown, _container);
            }
        }
    }
}
