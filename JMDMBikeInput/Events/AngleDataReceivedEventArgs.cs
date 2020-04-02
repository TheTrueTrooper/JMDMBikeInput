using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMDMMountainBikeInput
{
    public class AngleDataReceivedEventArgs : EventArgs
    {
        public short Angle { get; private set; }

        internal AngleDataReceivedEventArgs(short Angle)
        {
            this.Angle = Angle;
        }
    }
}
