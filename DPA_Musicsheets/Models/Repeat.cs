using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;
using DPA_Musicsheets.Converter.Midi;

namespace DPA_Musicsheets.Models
{
    public class Repeat : Token
    {
        public Block block;
        public int repeat;
        public Block toRepeat;

        public Repeat(Block block, int repeat)
        {
            this.block = block;
            this.repeat = repeat;
            this.toRepeat = new Block();
        }

        public Repeat(Block block, int repeat, Block toRepeat)
        {
            this.block = block;
            this.repeat = repeat;
            this.toRepeat = toRepeat;
        }

        public void AcceptLily(LilyVisitor v)
        {
            v.Visit(this);
        }
    }
}
