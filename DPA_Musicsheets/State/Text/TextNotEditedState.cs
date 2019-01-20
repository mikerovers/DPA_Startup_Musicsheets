using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.State
{
    public class TextNotEditedState : TextEditState
    {
        public TextNotEditedState(ICanHaveTextEditState context) : base(context)
        {
        }

        public override void Exit()
        {
            // Do nothing.
        }
    }
}
