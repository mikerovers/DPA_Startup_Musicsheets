using DPA_Musicsheets.Converter;
using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Events;
using DPA_Musicsheets.Models;
using DPA_Musicsheets.State;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void OpenFile(string fileName)
        {
            var fromConverterFactory = new FromConverterFactory();
            var converter = fromConverterFactory.GetConverter(Path.GetExtension(fileName));

            if (converter == null)
            {
                throw new NotSupportedException($"File extension {Path.GetExtension(fileName)} is not supported.");
            }

            Block = converter.ConvertToFromFile(fileName);
            state = new TextNotEditedState(this);
        }
    }
}
