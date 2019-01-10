using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Chain
{
    public interface IShortcutHandler
    {
        IShortcutHandler setNext(IShortcutHandler handler);
        object Handle(List<System.Windows.Input.Key> keyDown, BlockContainer _container);
    }
}
