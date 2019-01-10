using DPA_Musicsheets.Converter;
using DPA_Musicsheets.Converter.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public interface Token
    {
        void AcceptLily(LilyVisitor v);
    }
}
