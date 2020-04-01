using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMDMMountainBikeInput
{
    public class InputDataReceivedEventArgs : EventArgs
    {
        public static Dictionary<string, byte> ButtonMap = null;

        public byte CurrentInputWordState { get; private set; }

        public bool this[byte ButtonNumber]
        {
            get
            {
                switch (ButtonNumber)
                {
                    case 0:
                        return (CurrentInputWordState & 0x01) != 0;
                    case 1:
                        return (CurrentInputWordState & 0x02) != 0;
                    case 2:
                        return (CurrentInputWordState & 0x04) != 0;
                    case 3:
                        return (CurrentInputWordState & 0x08) != 0;
                    case 4:
                        return (CurrentInputWordState & 0x10) != 0;
                    case 5:
                        return (CurrentInputWordState & 0x20) != 0;
                    case 6:
                        return (CurrentInputWordState & 0x40) != 0;
                    case 7:
                        return (CurrentInputWordState & 0x80) != 0;
                    default:
                        throw new IndexOutOfRangeException("There are only a posiblity of 8 digital values.\nPlease observe a Zero based indexing style.");
                }
            }
        }

        public bool this[string ButtonNameOnMap]
        {
            get
            {
                if (ButtonMap == null)
                    throw new IndexOutOfRangeException("Button map is not set and no buttons have been mapped.\nHave you loaded the appropreate map into the static dictionary 'InputEventArgs.ButtonMap' for mapping?");
                if (!ButtonMap.ContainsKey(ButtonNameOnMap))
                    throw new IndexOutOfRangeException("Button has not been mapped.\nHave you loaded the appropreate map into the static dictionary 'InputEventArgs.ButtonMap' for mapping?");
                return this[ButtonMap[ButtonNameOnMap]];
            }
        }

        internal InputDataReceivedEventArgs(byte CurrentInputWordState)
        {
            this.CurrentInputWordState = CurrentInputWordState;
        }

    }
}
