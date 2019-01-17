using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Managers;

namespace DPA_Musicsheets.Commands
{
    class NullCommand : IShortcutCommand
    {
        public string pattern => "null";

        public void Execute(BlockContainer container)
        {}
    }
}
