using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Converter;

namespace DPA_Musicsheets.Models
{
    class Bar : Token
    {
        public void AcceptLily(LilyVisitor v)
        {
            v.Visit(this);
        }
    }
}
