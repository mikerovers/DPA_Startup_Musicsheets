using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter
{
    interface LilyVisitor
    {
        void Visit(Block block);
        void Visit(TimeSignature timeSignature);
        void Visit(Tempo tempo);
        void Visit(Note note);
        void Visit(Clef clef);
        void Visit(Rest rest);
        void Visit(Bar bar);
    }

    class ToLilyVisitor : LilyVisitor
    {
        private StringBuilder stringBuilder;

        public ToLilyVisitor(StringBuilder stringBuilder)
        {
            this.stringBuilder = stringBuilder;
        }

        public void Visit(Block block)
        {
            stringBuilder.Append("\\relative c' {");
            stringBuilder.AppendLine();
            foreach (var token in block)
            {
                token.AcceptLily(this);    
            }
            stringBuilder.Append("}");
        }

        public void Visit(TimeSignature timeSignature)
        {
            stringBuilder.Append($@"\time {timeSignature.before}/{timeSignature.after}");
            stringBuilder.AppendLine();
        }

        public void Visit(Tempo tempo)
        {
            stringBuilder.Append($@"\tempo 4={60000000 / tempo.tempo}");
            stringBuilder.AppendLine();
        }

        public void Visit(Note note)
        {
            stringBuilder.Append($"{note.key}{note.length}");
            stringBuilder.Append(" ");
        }

        public void Visit(Clef clef)
        {
            stringBuilder.Append($@"\clef {clef.type}");
            stringBuilder.AppendLine();
        }

        public void Visit(Rest rest)
        {
            stringBuilder.Append("r");
        }

        public void Visit(Bar bar)
        {
            stringBuilder.Append("|");
            stringBuilder.AppendLine();
        }
    }
}
