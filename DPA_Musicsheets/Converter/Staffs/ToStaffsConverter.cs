using DPA_Musicsheets.Models;
using PSAMControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Staffs
{
    class ToStaffsConverter : IToConverter<IList<MusicalSymbol>>
    {
        public IList<MusicalSymbol> ConvertTo(Block block)
        {
            return new List<MusicalSymbol>();
        }
    }
}
