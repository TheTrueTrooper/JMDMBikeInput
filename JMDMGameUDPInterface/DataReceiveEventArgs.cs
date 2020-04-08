using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMDMGameUDPInterface
{
    /// <summary>
    /// An event argument to handle the Data received
    /// </summary>
    public class DataReceiveEventArgs : EventArgs
    {
        public byte[] Data { get; private set; }

        public DateTime DateTime = DateTime.UtcNow;

        internal DataReceiveEventArgs(byte[] Data)
        {
            this.Data = Data;
        }
    }
}
