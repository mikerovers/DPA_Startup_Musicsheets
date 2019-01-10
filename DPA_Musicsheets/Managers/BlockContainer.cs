using DPA_Musicsheets.Events;
using DPA_Musicsheets.Models;
using DPA_Musicsheets.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Managers
{
    public class BlockContainer : ICanHaveTextEditState
    {
        private Stack<Block> undo;
        private Stack<Block> redo;
        
        private bool IsRedoingOrUndoing;

        public int CarotIndex { get; set; }

        private Block block;
        public Models.Block Block {
            get { return block; }
            set
            {
                if (block != null && !IsRedoingOrUndoing)
                {
                    undo.Push(block);
                    redo.Clear();
                }

                block = value;
                TextChanged.Invoke(this, new TextChangedEventArgs() { block = value });
                state = new TextEditedState(this);
                IsRedoingOrUndoing = false;
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (block != null && !IsRedoingOrUndoing)
                {
                    undo.Push(block);
                    redo.Clear();
                }

                _text = value;
                TextChanged.Invoke(this, new TextChangedEventArgs() { _text = value });
                state = new TextEditedState(this);
                IsRedoingOrUndoing = false;
            }
        }

        public TextEditState state;

        public event EventHandler<TextChangedEventArgs> TextChanged;

        public BlockContainer()
        {
            IsRedoingOrUndoing = false;
            undo = new Stack<Block>();
            redo = new Stack<Block>();
            state = new TextNotEditedState(this);
        }

        public bool CanUndoBlock()
        {
            return undo.Count > 0;
        }

        public bool CanRedoBlock()
        {
            return redo.Count > 0;
        }

        public void UndoBlock()
        {
            if (CanUndoBlock())
            {
                state = new TextEditedState(this);
                redo.Push(block);
                IsRedoingOrUndoing = true;
                Block = undo.Pop();
            }
        }

        public void RedoBlock()
        {
            if (CanRedoBlock())
            {
                state = new TextEditedState(this);
                undo.Push(block);
                IsRedoingOrUndoing = true;
                Block = redo.Pop();
            }
        }
    }
}
