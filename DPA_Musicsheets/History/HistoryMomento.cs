using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.History
{
    class HistoryMomento
    {
        private string _state;

        public HistoryMomento(string state)
        {
            this._state = state;
        }

        public string GetState()
        {
            return _state;
        }
    }
}
