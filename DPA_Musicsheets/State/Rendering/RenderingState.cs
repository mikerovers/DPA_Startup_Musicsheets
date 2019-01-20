using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.State.Render
{
    public abstract class RenderingState
    {
        public virtual bool IsRendering { get; }

        public virtual string StringToRender
        { get; }

        public override string ToString()
        {
            return StringToRender;
        }
    }
}
