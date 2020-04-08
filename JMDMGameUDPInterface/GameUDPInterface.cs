using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JMDMGameUDPInterface
{
    public class GameUDPInterface
    {
        /// <summary>
        /// A event delegate to handle the event
        /// </summary>
        /// <param name="Sender">a generalized object to hold the sender (rarely actually used except for loging)</param>
        /// <param name="e">the event arg with the Data for the event</param>
        public delegate void DataReceiveEventHandler(object Sender, DataReceiveEventArgs e);

        /// <summary>
        /// An event to handle the Received data
        /// </summary>
        public event DataReceiveEventHandler DataReceiveEvent;

        public int? ReceiveFromGamesInputPort
        {
            get
            {
                return ThisIPEndPoint?.Port;
            }
        }

        public int? SendToGamesInputPort
        {
            get
            {
                return GameIPEndPoint?.Port;
            }
        }

        public IPAddress ReceiveFromGamesInputAddress
        {
            get
            {
                return ThisIPEndPoint?.Address;
            }
        }

        public IPAddress SendToGamesInputAddress
        {
            get
            {
                return GameIPEndPoint?.Address;
            }
        }


        IPEndPoint GameIPEndPoint;
        IPEndPoint ThisIPEndPoint;

        UdpClient SendMessageToGameClient;
        UdpClient ReceiveMessageFromGameClient;

        Thread ListeningThread;

        public bool IsDisposed { get; private set; } = false;


        public GameUDPInterface(int ReceiveFromGamesInputPort, int SendToGamesInputPort, string ReceiveFromGamesInputPortIP = "127.0.0.1", string SendToGamesInputPortInputPortIP = "127.0.0.1")
        {
            //Create a end point to send data to a the game and bind it
            GameIPEndPoint = new IPEndPoint(IPAddress.Parse(SendToGamesInputPortInputPortIP), SendToGamesInputPort);
            SendMessageToGameClient = new UdpClient();

            //Create a end point to recieve data from the game and bind it.
            ThisIPEndPoint = new IPEndPoint(IPAddress.Parse(ReceiveFromGamesInputPortIP), ReceiveFromGamesInputPort);
            ReceiveMessageFromGameClient = new UdpClient(ThisIPEndPoint);
        }

        public void SendRawPacket(byte[] Data)
        {
            SendMessageToGameClient.Send(Data, Data.Length, GameIPEndPoint);
        }

        public void StartListenLoop()
        {
            ListeningThread = new Thread(new ThreadStart(ListenForDataLoop));
            ListeningThread.Start();
        }

        void ListenForDataLoop()
        {
            if (!IsDisposed)
            {
                //IPEndPoint Ref = ThisIPEndPoint;
                //byte[] ReceivedData = ReceiveMessageFromGameClient.Receive(ref Ref);
                byte[] ReceivedData = ReceiveMessageFromGameClient.Receive(ref ThisIPEndPoint);

                DataReceiveEvent.Invoke(this, new DataReceiveEventArgs(ReceivedData));
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                SendMessageToGameClient?.Dispose();
                ReceiveMessageFromGameClient?.Dispose();
            }
        }

        ~GameUDPInterface()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                SendMessageToGameClient?.Dispose();
                ReceiveMessageFromGameClient?.Dispose();
            }
        }
    }
}
