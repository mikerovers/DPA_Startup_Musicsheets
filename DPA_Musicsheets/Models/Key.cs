using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public class Key
    {
        public KeyType keyType;
        public int length;

        public Key(KeyType type, int length)
        {
            keyType = type;
            this.length = length;
        }

        public string KeyString()
        {
            if (keyType == KeyType.FLAT)
            {
                return "es"; 
            } else if (keyType == KeyType.SHARP)
            {
                return "is";
            } else
            {
                return "";
            }
        }
    }
}
