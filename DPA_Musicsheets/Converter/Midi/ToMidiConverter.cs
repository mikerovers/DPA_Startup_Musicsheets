using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Midi
{
    class ToMidiConverter : IToConverter<Sequence>
    {
        public Sequence ConvertTo(Block block)
        {
            return new Sequence();
        }
    }
}
