using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.Converter.Lilypond
{
    class FromLilypondConverter : IFromConverter
    {
        public Block ConvertTo(string fileName)
        {
            return new Block();
        }
    }
}
