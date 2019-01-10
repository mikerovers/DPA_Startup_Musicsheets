using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.State
{
    public abstract class TextEditState
    {
        public ICanHaveTextEditState Context { get; set; }
        public abstract void Exit();

        public TextEditState(ICanHaveTextEditState context)
        {
            this.Context = context;
        }
    }
}
