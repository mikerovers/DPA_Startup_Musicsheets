using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Chain
{
    public class InsertTimeHandler : AbstractShortcutHandler
    {
        private IShortcutCommand command;

        public override object Handle(List<System.Windows.Input.Key> keyDown, BlockContainer _container)
        {
            if (keyDown.Contains(System.Windows.Input.Key.LeftCtrl)
                && keyDown.Contains(System.Windows.Input.Key.T))
            {
                if (keyDown.Contains(System.Windows.Input.Key.D4))
                {
                    command = new InsertCommand("\\time 4/4 \n");
                }
                else if (keyDown.Contains(System.Windows.Input.Key.D3)) {
                    command = new InsertCommand("\\time 3/4 \n");
                }
                else if (keyDown.Contains(System.Windows.Input.Key.D6))
                {
                    command = new InsertCommand("\\time 6/8 \n");
                }
                else
                {
                    command = new InsertCommand("\\time 4/4 \n");
                }

                System.Console.WriteLine("Inputting timesignature.");

                command.Execute(_container);
                
                return "";
            }
            else
            {
                return base.Handle(keyDown, _container);
            }
        }
    }
}
