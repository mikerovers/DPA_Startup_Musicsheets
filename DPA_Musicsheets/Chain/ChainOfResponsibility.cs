using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.Chain
{
    class ChainOfResponsibility : AbstractShortcutHandler
    {
        private List<IShortcutHandler> _chain = new List<IShortcutHandler>();

        public void AddHandlerToChain(IShortcutHandler handler)
        {
            if (_chain.Count > 0) 
                _chain.Last().setNext(handler);

            _chain.Add(handler);
        }

        public override object Handle(List<Key> keyDown, BlockContainer _container)
        {
            return _chain.First().Handle(keyDown, _container);
        }
    }
}
