using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMDMMountainBikeInput
{
    public class AngelDataReceivedEventArgs : EventArgs
    {
        public short Angel { get; private set; }

        internal AngelDataReceivedEventArgs(short Angel)
        {
            this.Angel = Angel;
        }
    }
}
