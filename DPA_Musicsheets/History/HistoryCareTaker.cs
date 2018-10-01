using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.History
{
    class HistoryCareTaker
    {
        private Stack<HistoryMomento> _undoStack;
        private Stack<HistoryMomento> _redoStack;

        public HistoryCareTaker()
        {
            Reset();
        }

        public int UndoCount
        {
            get
            {
                return _undoStack.Count;
            }
        }

        public int RedoCount
        {
            get
            {
                return _redoStack.Count;
            }
        }

        public void Reset()
        {
            _undoStack = new Stack<HistoryMomento>();
            _redoStack = new Stack<HistoryMomento>();
        }

        public void AddMomento(HistoryMomento history)
        {
            _redoStack.Clear();
            _undoStack.Push(history);
        }

        public HistoryMomento Undo()
        {
            if (UndoCount > 0)
            {
                HistoryMomento h = _undoStack.Pop();
                _redoStack.Push(h);

                return h; 
            }

            return new HistoryMomento("");
        }

        public bool CanUndo()
        {
            return UndoCount > 0 ? true : false;
        }

        public HistoryMomento Redo()
        {
            if (RedoCount > 0)
            {
                HistoryMomento h = _redoStack.Pop();
                _undoStack.Push(h);

                return h;
            }

            return new HistoryMomento("");
        }

        public bool CanRedo()
        {
            return RedoCount > 0 ? true : false;
        }
    }
}
