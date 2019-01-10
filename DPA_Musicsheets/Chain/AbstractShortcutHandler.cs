using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Chain
{
    public class AbstractShortcutHandler : IShortcutHandler
    {
        private IShortcutHandler nextHandler;

        public virtual object Handle(List<System.Windows.Input.Key> keyDown, BlockContainer _container)
        {
            if (nextHandler != null)
            {
                return this.nextHandler.Handle(keyDown, _container);
            } else
            {
                return null;
            }
        }

        public IShortcutHandler setNext(IShortcutHandler handler)
        {
            this.nextHandler = handler;

            return handler;
        }
    }
}
