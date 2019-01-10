using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Commands
{
    public interface IShortcutCommand
    {
        string pattern { get; }

        void Execute(BlockContainer container);
    }
}
