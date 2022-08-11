using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace k204 {
    public class K204 {

        public K204() {
            
        }

        public List<TemperatureReading> Channel1Data = new();
        public List<TemperatureReading> Channel2Data = new();
        public List<TemperatureReading> Channel3Data = new();
        public List<TemperatureReading> Channel4Data = new();

        public bool IsRecMode { get; private set; }
        public bool IsModeNormal { get; private set; }
        public bool IsModeMinMax { get; private set; }
        public bool IsModeRel { get; private set; }
        public bool IsHoldMode { get; private set; }
        public bool IsBatteryLow { get; private set; }


        public bool IsMemoryFull { get; private set; }
        public bool IsModeAutoPowerOff { get; private set; }


        public static void LogBuffer(ref byte[] data) {
            int i = 0;
            foreach (var d in data) {
                var idx = i.ToString("D2");
                Console.Write("#" + idx + "    ");
                Console.Write("0x{00:X2}", d);
                Console.Write("    0b");
                string binary = Convert.ToString(d, 2);
                Console.Write(binary.PadLeft(8, '0'));
                //Console.Write("    ");
                //if (i == 7) { Console.Write("CH1 (a)"); }
                //if (i == 8) { Console.Write("CH1 (b)"); }
                Console.WriteLine("");
                ++i;
            }
        }

        public bool ParseK204DataBuffer(ref byte[] data) {
            LogBuffer(ref data);
            bool success = false;
            if (data.Length != 45) { goto bail; }

            // check header value
            if (data[0] != 0x02) { goto bail; }
            // check footer value
            if (data[44] != 0x03) { goto bail; }

            var ch1 = new TemperatureReading();
            success = ch1.ParseDataBufferAsChannel(0, ref data);
            if (!success) { goto bail; }
            Channel1Data.Add(ch1);

            var ch2 = new TemperatureReading();
            success = ch2.ParseDataBufferAsChannel(1, ref data);
            if (!success) { goto bail; }
            Channel2Data.Add(ch2);

            var ch3 = new TemperatureReading();
            success = ch3.ParseDataBufferAsChannel(2, ref data);
            if (!success) { goto bail; }
            Channel3Data.Add(ch3);

            var ch4 = new TemperatureReading();
            success = ch4.ParseDataBufferAsChannel(3, ref data);
            if (!success) { goto bail; }
            Channel4Data.Add(ch4);


        bail:
            return success;
        }
    }
}
