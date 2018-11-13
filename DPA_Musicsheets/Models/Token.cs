using DPA_Musicsheets.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    interface Token
    {
        void AcceptLily(LilyVisitor v);
    }
}
