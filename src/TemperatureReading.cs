using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace k204 {
    public class TemperatureReading {

        public TemperatureReading() {
            TimeStamp = DateTime.Now;
        }

        public DateTime TimeStamp { get; }
        public bool IsOL { get; private set; }

        public double TemperatureValue { get; private set; }
        public double TemperatureRel { get; private set; }
        public double TemperatureMin { get; private set; }
        public double TemperatureMax { get; private set; }

        // The data order from K204 is big endian
        public bool ParseDataBufferAsChannel(byte ch, ref byte[] data) {
            if (ch < 0 || ch > 3) { throw new Exception("Invalid channel number #" + ch + " must be between 0..3."); }
            if (data.Length == 0) { return false; }
            int offset = 2 * ch;
            int offsetch = 7 + offset;
            int offsetrel = 15 + offset;
            int offsetmin = 23 + offset;
            int offsetmax = 31 + offset;

            try {
                TemperatureValue = ConcatBytes(data[offsetch], data[offsetch + 1]);
                TemperatureRel = ConcatBytes(data[offsetrel], data[offsetrel + 1]);
                TemperatureMin = ConcatBytes(data[offsetmin], data[offsetmin + 1]);
                TemperatureMax = ConcatBytes(data[offsetmax], data[offsetmax + 1]);

                // check for OL of 40th byte
                var olCheck = data[39];
                IsOL = IsBitSet(olCheck, ch);

                // resolution
                var resCheck = data[43];
                if (IsBitClear(resCheck, ch)) { TemperatureValue /= 10; }

                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// Parses two big endian bytes into a 16 bit short int
        /// </summary>
        /// <param name="h">high order byte</param>
        /// <param name="l">low order byte</param>
        /// <returns>a short int considering host endianness</returns>
        static short ConcatBytes(byte h, byte l) {
            var buf = new byte[2] { l, h };
            if (!BitConverter.IsLittleEndian) {
                buf.Reverse();
            }
            return BitConverter.ToInt16(buf, 0);
        }

        static bool IsBitSet(byte b, byte i) {
            var mask = 1 << i;
            return (b & mask) > 0;
        }

        static bool IsBitClear(byte b, byte i) {
            var mask = 1 << i;
            return (b & mask) == 0;
        }
    }
}
