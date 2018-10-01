using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class EndOfTrackSignature : MusicalSymbol
    {
        private Models.TimesSignature _timesSignature;

        public EndOfTrackSignature(Models.TimesSignature timesSignature)
        {
            this._timesSignature = _timesSignature;
        }

        public override string toLilyPond()
        {
            StringBuilder builder = new StringBuilder();

            return builder.ToString();
        }
    }
}
