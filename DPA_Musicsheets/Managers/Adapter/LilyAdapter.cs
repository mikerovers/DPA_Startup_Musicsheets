using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Managers.Adapter
{
    class LilyAdapter : FileLoader
    {
        private LilyPondLoader adaptee;

        public LilyAdapter(LilyPondLoader adaptee)
        {
            this.adaptee = adaptee;
        }

        public string LoadFile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
