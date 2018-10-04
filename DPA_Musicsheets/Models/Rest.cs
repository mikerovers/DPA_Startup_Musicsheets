using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class Rest: Token
    {
        public Duration duration;

        public Rest(Duration duraction)
        {
            this.duration = duraction;
        }
    }
}
