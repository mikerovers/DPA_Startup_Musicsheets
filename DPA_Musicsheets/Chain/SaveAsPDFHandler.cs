using DPA_Musicsheets.Commands.Export;
using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Chain
{
    class SaveAsPDFHandler : AbstractShortcutHandler
    {
        private PDFExportCommand pdfExportCommand = new PDFExportCommand();

        public override object Handle(List<System.Windows.Input.Key> keyDown, BlockContainer _container)
        {
            if (
                keyDown.Contains(System.Windows.Input.Key.LeftCtrl)
                && keyDown.Contains(System.Windows.Input.Key.P)
                )
            {
                System.Console.WriteLine("Saving as PDF.");
                pdfExportCommand.Execute(_container);
                return "";
            }
            else
            {
                return base.Handle(keyDown, _container);
            }
        }
    }
}
