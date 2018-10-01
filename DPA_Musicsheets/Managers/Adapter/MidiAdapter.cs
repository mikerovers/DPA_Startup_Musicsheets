using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Managers
{
    class MidiAdapter : FileLoader
    {
        private MidiFileLoader adaptee;

        public MidiAdapter(MidiFileLoader adaptee)
        {
            this.adaptee = adaptee;
        }

        public string LoadFile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
