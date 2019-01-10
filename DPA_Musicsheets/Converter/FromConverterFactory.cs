using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Converter.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter
{
    public class FromConverterFactory
    {
        private Dictionary<string, IFromConverter> converters;

        public FromConverterFactory()
        {
            converters = new Dictionary<string, IFromConverter>();
            converters.Add(".mid", new FromMidiConverter());
            converters.Add(".ly", new FromLilypondConverter());
        }

        public IFromConverter GetConverter(string fileType)
        {
            if (converters.ContainsKey(fileType))
            {
                return converters[fileType];
            }

            return null;
        }
    }
}
