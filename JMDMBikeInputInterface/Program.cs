using System;
using System.IO;
using JMDMMountainBikeInput;
using JMDMBikeGameUDPInterfaceNS;
using System.IO.Ports;
using JMDMGameUDPInterface;
using System.Text;

namespace JMDMBikeInputInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Config Config = Config.LoadConfig();
            MountBikeInput_Com ComInput = new MountBikeInput_Com(Config.InputControlsComPort);
            SerialPort MoveCom = new SerialPort(Config.OutputServossComPort, 9600, Parity.None, 8, StopBits.One);
            JMDMBikeGameUDPInterface GameOutput = new JMDMBikeGameUDPInterface(SendToGamesInputPort: Config.SendToGameUDPPort, ReceiveFromGamesInputPort: Config.ReceiveFromGameUDPPort);
            short AngleCal = Config.AngleCalibration;

            ComInput.AngleDataReceivedEvent += (object sender, AngleDataReceivedEventArgs e) => 
            {
                Console.WriteLine($"FromJMDMBoard:Recived:A({e.Angle.ToString("0000")})");
                GameOutput.SendAngle((short)(e.Angle + AngleCal));
                Console.WriteLine($"ToGame:Sent:S2({(e.Angle + AngleCal).ToString("0000")})");
            };
            ComInput.SpeedDataReceivedEvent += (object sender, SpeedDataReceivedEventArgs e) => 
            {
                Console.WriteLine($"FromJMDMBoard:Recived:S({e.Speed.ToString("000")})");
                short Calc = (short)(1000 - e.Speed);
                GameOutput.SendSpeed(Calc);
                Console.WriteLine($"ToGame:Sent:S1({Calc.ToString("0000")})");
            };
            GameOutput.MoveCommandReceivedEvent += (object sender, MoveCommandReceivedEventArgs e) =>
            {
                Console.WriteLine($"FromGame:Recived:O2({e.Tilt.ToString("000")})");
                MoveCom.Write($"CO(01,{e.Tilt.ToString("000")})");
                Console.WriteLine($"ToJMDMBoard:Recived:CO(01,{ e.Tilt.ToString("000")})");
            };
            ComInput.OpenPortAndListen();
            MoveCom.Open();
            GameOutput.StartListenLoop();
            Console.ReadKey();
        }
    }
}
