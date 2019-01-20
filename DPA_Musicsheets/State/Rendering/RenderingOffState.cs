using DPA_Musicsheets.State.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.State.Rendering
{
    class RenderingOffState : RenderingState
    {
        public override bool IsRendering => false;
        public override string StringToRender => "";
    }
}
