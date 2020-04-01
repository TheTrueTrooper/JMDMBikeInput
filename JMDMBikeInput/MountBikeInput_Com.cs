using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace JMDMMountainBikeInput
{
    public class MountBikeInput_Com : IDisposable
    {
        public delegate void InputDataReceivedEventHandler(object sender, InputDataReceivedEventArgs e);
        public delegate void AngelDataReceivedEventHandler(object sender, AngelDataReceivedEventArgs e);
        public delegate void SpeedDataReceivedEventHandler(object sender, SpeedDataReceivedEventArgs e);

        public InputDataReceivedEventHandler InputDataReceivedEvent;
        public AngelDataReceivedEventHandler AngelDataReceivedEvent;
        public SpeedDataReceivedEventHandler SpeedDataReceivedEvent;

        internal SerialPort InputListenCom { set; get; }

        internal string ListenPort { private set; get; }

        bool Disposed = false;

        Thread ListenerThread;

        internal bool IsOpen
        {
            get => InputListenCom.IsOpen;
        }

        public MountBikeInput_Com(string ListenPort)
        {
            this.ListenPort = ListenPort;
            InputListenCom = new SerialPort(ListenPort, 9600, Parity.None, 8, StopBits.One);
            InputListenCom.ReadTimeout = 400;

        }

        public void OpenPortAndListen()
        {
            if (!InputListenCom.IsOpen)
            {
                InputListenCom.Open();
                ListenerThread = new Thread(new ParameterizedThreadStart(Listening));
                ListenerThread.Start(this);
            }
        }

        void Listening(object ThisIn)
        {
            MountBikeInput_Com This = (MountBikeInput_Com)ThisIn;
            bool Disposed = false;
            lock (This)
                Disposed = This.Disposed;
            while (!Disposed)
            {
                //acuire a message
                //start the message
                char[] MessageType = new char[14];
                lock(This)
                    MessageType[0] = (char)This.InputListenCom.ReadByte();
                int Count = 1;
                //then read bytes until message done
                do
                {
                    lock (This)
                        MessageType[Count] = (char)This.InputListenCom.ReadByte();
                    Count++;
                }
                while (MessageType[Count - 1] != ')');
                //Dispach message
                switch(MessageType[0])
                {
                    case 'I':
                        {
                            byte InputWord = 0;
                            if (MessageType[2] != '0')
                                InputWord |= 0x01;
                            if (MessageType[3] != '0')
                                InputWord |= 0x02;
                            if (MessageType[4] != '0')
                                InputWord |= 0x04;
                            if (MessageType[5] != '0')
                                InputWord |= 0x08;
                            if (MessageType[6] != '0')
                                InputWord |= 0x10;
                            if (MessageType[7] != '0')
                                InputWord |= 0x20;
                            if (MessageType[8] != '0')
                                InputWord |= 0x40;
                            if (MessageType[9] != '0')
                                InputWord |= 0x80;
                            InputDataReceivedEvent.Invoke(this, new InputDataReceivedEventArgs(InputWord));
                        }
                        break;
                    case 'A':
                        {
                            string Value = "";
                            for (byte i = 2; i < 7; i++)
                                Value += MessageType[i];
                            AngelDataReceivedEvent.Invoke(this, new AngelDataReceivedEventArgs(short.Parse(Value)));
                        }
                        break;
                    case 'S':
                        {
                            string Value = "";
                            for (byte i = 2; i < 5; i++)
                                Value += MessageType[i];
                            SpeedDataReceivedEvent.Invoke(this, new SpeedDataReceivedEventArgs(short.Parse(Value)));
                        }
                        break;
                }
                lock (This)
                    Disposed = This.Disposed;
            }
        }

        public void Dispose()
        {
            if(!Disposed)
            {
                Disposed = true;
                ListenerThread.Abort();
                InputListenCom.Dispose();
            }
        }

        ~MountBikeInput_Com()
        {
            if (!Disposed)
            {
                Disposed = true;
                ListenerThread?.Abort();
                InputListenCom?.Dispose();
            }
        }
    }
}
