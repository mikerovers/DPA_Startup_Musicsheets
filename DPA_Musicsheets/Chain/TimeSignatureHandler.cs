using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Chain
{
    public class TimeSignatureHandler : AbstractShortcutHandler
    {
        private InsertTimeCommand insertTimeCommand = new InsertTimeCommand();

        public override object Handle(List<System.Windows.Input.Key> keyDown, BlockContainer _container)
        {
            if (
                keyDown.Contains(System.Windows.Input.Key.LeftCtrl)
                && keyDown.Contains(System.Windows.Input.Key.H)
                )
            {
                System.Console.WriteLine("Inputting timesignature.");
                insertTimeCommand.Execute(_container);
                return "";
            }
            else
            {
                return base.Handle(keyDown, _container);
            }
        }
    }
}
