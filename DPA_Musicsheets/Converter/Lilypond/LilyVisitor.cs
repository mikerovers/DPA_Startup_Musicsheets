﻿using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter
{
    public interface LilyVisitor
    {
        void Visit(Block block);
        void Visit(TimeSignature timeSignature);
        void Visit(NullToken nullToken);
        void Visit(Tempo tempo);
        void Visit(Note note);
        void Visit(Clef clef);
        void Visit(Rest rest);
        void Visit(Bar bar);
        void Visit(Repeat repeat);
    }

    public class ToLilyVisitor : LilyVisitor
    {
        private StringBuilder stringBuilder;

        public ToLilyVisitor(StringBuilder stringBuilder)
        {
            this.stringBuilder = stringBuilder;
        }

        public void Visit(Block block)
        {
            if (block != null)
                foreach (var token in block)
                {
                    this.Visit(token as dynamic);
                }
        }

        public void Visit(TimeSignature timeSignature)
        {
            stringBuilder.Append($@"\time {timeSignature.before}/{timeSignature.after}");
            stringBuilder.AppendLine();
        }

        public void Visit(Tempo tempo)
        {
            stringBuilder.Append($@"\tempo {tempo.upper}={tempo.downer}");
            stringBuilder.AppendLine();
        }

        public void Visit(Note note)
        {
            var key = note.key.KeyString();
            var octave = note.OctaveString();
            var pitch = note.pitch.ToString().ToLower();
            stringBuilder.Append($"{pitch}{key}{octave}{note.Length}");
            stringBuilder.Append(" ");
        }

        public void Visit(Clef clef)
        {
            stringBuilder.AppendLine($@"\clef {clef.type}");
        }

        public void Visit(Rest rest)
        {
            stringBuilder.Append($"r{rest.Duration}");
            stringBuilder.AppendLine();
        }

        public void Visit(Bar bar)
        {
            stringBuilder.Append("|");
            stringBuilder.AppendLine();
        }

        public void Visit(Repeat repeat)
        {
            stringBuilder.Append($@"\repeat volta {repeat.repeat} ");
            stringBuilder.Append("{");
            stringBuilder.AppendLine();
            Visit(repeat.block);
            stringBuilder.Append("}");
            stringBuilder.AppendLine();
            if (repeat.toRepeat.Count() > 0)
            {
                stringBuilder.Append($@"\alternative {{");
                stringBuilder.AppendLine();
                foreach(Token token in repeat.toRepeat)
                {
                    if (token is Block)
                        stringBuilder.Append(@"{ ");

                    Visit(token as dynamic);

                    if (token is Block)
                    {
                        stringBuilder.Append("}");
                        stringBuilder.AppendLine();
                    }
                }
                stringBuilder.Append(@"}");
                stringBuilder.AppendLine();
            }
        }

        public void Visit(NullToken nullToken)
        {
            // Do nothing :)
        }
    }
}
