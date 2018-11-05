using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converter.Midi
{
    class MidiMessageConverterFactory
    {
        Dictionary<MessageType, IMidiMessageConverter> dictionary;

        public MidiMessageConverterFactory()
        {
            dictionary = new Dictionary<MessageType, IMidiMessageConverter>();
            this.dictionary.Add(MessageType.Meta, new MetaMessageConverter());
            this.dictionary.Add(MessageType.Channel, new ChannelMessageConverter());
        }

        public IMidiMessageConverter GetConverter(MessageType messageType)
        {
            if (dictionary.ContainsKey(messageType))
            {
                return dictionary[messageType];
            }

            return null;
        }
    }
}
