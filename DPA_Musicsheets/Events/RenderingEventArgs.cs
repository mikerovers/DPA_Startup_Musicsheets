using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.State.Rendering
{
    public class RenderingEventArgs : EventArgs
    {
        public RenderingEventArgs(bool IsRendering, string StringToPrint)
        {
            this.IsRendering = IsRendering;
            this.StringToPrint = StringToPrint;
        }

        public bool IsRendering { get; private set; }
        public string StringToPrint { get; private set; }
    }
}
