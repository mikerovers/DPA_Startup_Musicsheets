using DPA_Musicsheets.Converter;
using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Events;
using DPA_Musicsheets.Models;
using DPA_Musicsheets.State;
using DPA_Musicsheets.State.Render;
using DPA_Musicsheets.State.Rendering;
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
                textEditState = new TextEditedState(this);
                IsRedoingOrUndoing = false;
            }
        }

        public RenderingState RenderState
        {
            get { return renderingState; }
            set
            {
                renderingState = value;
                RenderingChanged.Invoke(this, new RenderingEventArgs() { IsRendering = renderingState.IsRendering });
            }
        }

        public TextEditState textEditState;
        private RenderingState renderingState;

        public event EventHandler<TextChangedEventArgs> TextChanged;
        public event EventHandler<RenderingEventArgs> RenderingChanged;

        public BlockContainer()
        {
            IsRedoingOrUndoing = false;
            undo = new Stack<Block>();
            redo = new Stack<Block>();
            textEditState = new TextNotEditedState(this);
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
                textEditState = new TextEditedState(this);
                redo.Push(block);
                IsRedoingOrUndoing = true;
                Block = undo.Pop();
            }
        }

        public void RedoBlock()
        {
            if (CanRedoBlock())
            {
                textEditState = new TextEditedState(this);
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
            textEditState = new TextNotEditedState(this);
        }
    }
}
