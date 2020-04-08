using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using JMDMGameUDPInterface;
using static JMDMGameUDPInterface.GameUDPInterface;

namespace JMDMBikeGameUDPInterfaceNS
{
    public class JMDMBikeGameUDPInterface : IDisposable
    {

        /// <summary>
        /// A event delegate to handle the event
        /// </summary>
        /// <param name="Sender">a generalized object to hold the sender (rarely actually used except for loging)</param>
        /// <param name="e">the event arg with the Data for the event</param>
        public delegate void MoveCommandReceivedEventHandler(object Sender, MoveCommandReceivedEventArgs e);

        /// <summary>
        /// An event to handle the Received data
        /// </summary>
        public event MoveCommandReceivedEventHandler MoveCommandReceivedEvent;

        /// <summary>
        /// An event to handle the Received data
        /// </summary>
        public event DataReceiveEventHandler DataReceiveEvent
        {
            add => UDPGameConnections.DataReceiveEvent += value;
            remove => UDPGameConnections.DataReceiveEvent += value;
        }

        /// <summary>
        /// the Port that the game will send info to this.
        /// </summary>
        public int? ReceiveFromGamesInputPort
        {
            get
            {
                return UDPGameConnections?.ReceiveFromGamesInputPort;
            }
        }

        public int? SendToGamesInputPort
        {
            get
            {
                return UDPGameConnections?.SendToGamesInputPort;
            }
        }

        public IPAddress ReceiveFromGamesInputAddress
        {
            get
            {
                return UDPGameConnections?.ReceiveFromGamesInputAddress;
            }
        }

        public IPAddress SendToGamesInputAddress
        {
            get
            {
                return UDPGameConnections?.SendToGamesInputAddress;
            }
        }

        GameUDPInterface UDPGameConnections;

        public bool IsDisposed { get; private set; } = false;


        public JMDMBikeGameUDPInterface(int ReceiveFromGamesInputPort = 7001, int SendToGamesInputPort = 6001, string ReceiveFromGamesInputPortIP = "127.0.0.1", string SendToGamesInputPortInputPortIP = "127.0.0.1")
        {
            UDPGameConnections = new GameUDPInterface(ReceiveFromGamesInputPort, SendToGamesInputPort, ReceiveFromGamesInputPortIP, SendToGamesInputPortInputPortIP);
            DataReceiveEvent += SelfhandleDataReceiveEvent;
        }

        public void StartListenLoop()
        {
            UDPGameConnections.StartListenLoop();
        }

        private void SelfhandleDataReceiveEvent(object Sender, DataReceiveEventArgs e)
        {
            const string MoveOrderConst = "O2(";
            string Message = ASCIIEncoding.ASCII.GetString(e.Data);

            if(Message.Contains(MoveOrderConst))
            {
                string MoveOrder = Message.Substring(Message.IndexOf(MoveOrderConst), 7);
                MoveCommandReceivedEvent.Invoke(this, new MoveCommandReceivedEventArgs(byte.Parse(MoveOrder.Substring(0,3))));
            }
        }
        


        void SendRawPacket(ref byte[] Data)
        {
            UDPGameConnections.SendRawPacket(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Angel">S1 (XXXX) The degree of deviation of the bicycle head. S1 (2500) is the center position, and the range is S1 (0000)-S1 (5000)</param>
        public void SendAngle(short Angel)
        {
            const string Message = "S1({0:0000})";
            byte[] Data = ASCIIEncoding.ASCII.GetBytes(string.Format(Message, Angel));
            SendRawPacket(ref Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Speed">S2 (XXXX) Bicycle speed data Explanation: S2 (0050) speed degree is 0; S2 (0051)-S2 (0995) speed degree is 220 * k / (1000- XXXX) k is the limit coefficient;S2 (0995)-S2 (1000) Discard</param>
        public void SendSpeed(short Speed)
        {
            const string Message = "S2({0:0000})";
            byte[] Data = ASCIIEncoding.ASCII.GetBytes(string.Format(Message, Speed));
            SendRawPacket(ref Data);
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                UDPGameConnections?.Dispose();
            }
        }

        ~JMDMBikeGameUDPInterface()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                UDPGameConnections?.Dispose();
            }
        }
    }
}
