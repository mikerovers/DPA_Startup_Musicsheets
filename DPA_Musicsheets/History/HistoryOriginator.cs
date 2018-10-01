using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.History
{
    class HistoryOriginator
    {
        private string _state;

        public void SetState(string state)
        {
            this._state = state;
        }

        public string GetState()
        {
            return this._state;
        }

        public HistoryMomento Save()
        {
            return new HistoryMomento(this._state);
        }

        public void Restore(HistoryMomento momento)
        {
            this._state = momento.GetState();
        }
    }
}
