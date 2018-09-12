using DPA_Musicsheets.Managers;
using DPA_Musicsheets.Managers.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Factory
{
    class FileLoaderFactory
    {
        private Dictionary<string, FileLoader> loaderMap;

        public FileLoaderFactory()
        {
            this.loaderMap = new Dictionary<string, FileLoader>();
            this.RegisterLoader(".mid", new MidiAdapter(new MidiFileLoader()));
            this.RegisterLoader(".ly", new LilyAdapter(new LilyPondLoader()));
        }

        public void RegisterLoader(string name, FileLoader loader)
        {
            if (!this.loaderMap.ContainsKey(name))
            {
                this.loaderMap[name] = loader;
            }
        }

        public FileLoader GetLoader(string name)
        {
            if (this.loaderMap.ContainsKey(name)) {
                return this.loaderMap[name];
            }

            throw new NotSupportedException($"File extension of file {name} is not supported.");
        }
    }
}
