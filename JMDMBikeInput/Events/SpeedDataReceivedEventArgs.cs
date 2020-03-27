using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMDMMountainBikeInput
{
    public class SpeedDataReceivedEventArgs : EventArgs
    {
        public short Speed { get; private set; }

        internal SpeedDataReceivedEventArgs(short Speed)
        {
            this.Speed = Speed;
        }
    }
}
