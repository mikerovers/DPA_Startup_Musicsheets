using DPA_Musicsheets.State.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.State.Rendering
{
    class RenderingOnState : RenderingState
    {
        public override bool IsRendering => true;
        public override string StringToRender => "Rendering...";
    }
}
