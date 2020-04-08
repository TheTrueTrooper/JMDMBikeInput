using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace JMDMBikeInputInterface
{
    class Config
    {
        public static Config LoadConfig()
        { 
            string FileName = "Config.json";
            using (StreamReader SR = new StreamReader($"{Environment.CurrentDirectory}\\{FileName}"))
            {
                JsonSerializer Deserializer = new JsonSerializer();
                return (Config)Deserializer.Deserialize(SR, typeof(Config));
            }
        }

        public string InputControlsComPort { get; set; }
        public string OutputServossComPort { get; set; }
        public int SendToGameUDPPort { get; set; }
        public int ReceiveFromGameUDPPort { get; set; }
        public short AngleCalibration { get; set; }
    }
}
