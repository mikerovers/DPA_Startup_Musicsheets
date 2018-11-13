using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter
{
    interface IToConverter<out T>
    {
        T ConvertTo(Block block);
    }
}
