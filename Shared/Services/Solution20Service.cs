using System.Text;

namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/20.txt
    public class Solution20Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(20, example);
            string originalString = string.Join(string.Empty, lines);
            byte[] allBytes = Convert.FromBase64String(originalString);
            // Skip BOM 0xFF 0xFE which is for UTF-16 little endian
            byte[] bytes = allBytes.Skip(2).ToArray();

            // Chunk into pairs of byte and flip the bytes to big endian
            List<List<byte>> bytePairs = bytes.Chunk(2).Select(c => new List<byte>(){c[1], c[0]}).ToList();

            // Convert to string of unicode values
            uint? highSurrogate = null;
            uint? lowSurrogate = null;

            string unicodeJoined = string.Empty;

            foreach (List<byte> pair in bytePairs) {
                string hex = Convert.ToString(pair[0], 16).PadLeft(2, '0') + Convert.ToString(pair[1], 16).PadLeft(2, '0');
                uint value = Convert.ToUInt32(hex, 16);

                if (value >= 0xD800 && value <= 0xDFFF) {
                    if (highSurrogate == null) {
                        highSurrogate = value;
                    }
                    else {
                        lowSurrogate = value;
                    }
                }
                else {
                    string surrogateString = Convert.ToString(value, 16).PadLeft(5, '0');
                    unicodeJoined += surrogateString;
                }

                if (highSurrogate != null && lowSurrogate != null) {
                    uint surrogateValue = 0x10000 + ((highSurrogate!.Value - 0xD800) << 10) + (lowSurrogate!.Value - 0xDC00);
                    string surrogateString = Convert.ToString(surrogateValue, 16).PadLeft(5, '0');
                    unicodeJoined += surrogateString;

                    highSurrogate = null;
                    lowSurrogate = null;
                }
            }

            byte[] unicodeBytes = unicodeJoined.Chunk(2).Select(x => Convert.ToByte(new string(x), 16)).ToArray();

            // Decode UTF-8, but support 5 and 6 byte encoding
            int remainingBytes = 0;
            List<bool> runningBits = [];

            List<string> utf8Characters = [];

            foreach (byte b in unicodeBytes) {
                if (remainingBytes > 0) {
                    byte compareBit = 0b00100000;

                    while (compareBit > 0) {
                        runningBits.Add((byte)(b & compareBit) == compareBit);
                        compareBit = (byte)(compareBit >> 1);
                    }

                    remainingBytes--;
                }
                else {
                    if (runningBits.Count > 0) {
                        string binary = string.Join(string.Empty, runningBits.Select(b => b ? '1' : '0')).PadLeft((int)Math.Ceiling(runningBits.Count / 4.0) * 4, '0');
                        string character = string.Join(string.Empty, binary.Chunk(4).Select(b => Convert.ToString(Convert.ToByte(new string(b), 2), 16))).TrimStart('0').PadLeft(7, '0');
                        utf8Characters.Add(character);
                        runningBits = [];
                    }

                    if (b == 0b11111111) {
                        remainingBytes = 7;
                    }
                    else if (b >= 0b11111110) {
                        remainingBytes = 6;
                    }
                    else if (b >= 0b11111100) {
                        remainingBytes = 5;
                        
                        runningBits.Add((byte)(b & 0b00000001) == 0b00000001);
                    }
                    else if (b >= 0b11111000) {
                        remainingBytes = 4;
                        
                        runningBits.Add((byte)(b & 0b00000010) == 0b00000010);
                        runningBits.Add((byte)(b & 0b00000001) == 0b00000001);
                    }
                    else if (b >= 0b11110000) {
                        remainingBytes = 3;
                        
                        runningBits.Add((byte)(b & 0b00000100) == 0b00000100);
                        runningBits.Add((byte)(b & 0b00000010) == 0b00000010);
                        runningBits.Add((byte)(b & 0b00000001) == 0b00000001);
                    }
                    else if (b >= 0b11100000) {
                        remainingBytes = 2;
                        
                        runningBits.Add((byte)(b & 0b00001000) == 0b00001000);
                        runningBits.Add((byte)(b & 0b00000100) == 0b00000100);
                        runningBits.Add((byte)(b & 0b00000010) == 0b00000010);
                        runningBits.Add((byte)(b & 0b00000001) == 0b00000001);
                    }
                    else if (b >= 0b11000000) {
                        remainingBytes = 1;
                        
                        runningBits.Add((byte)(b & 0b00010000) == 0b00010000);
                        runningBits.Add((byte)(b & 0b00001000) == 0b00001000);
                        runningBits.Add((byte)(b & 0b00000100) == 0b00000100);
                        runningBits.Add((byte)(b & 0b00000010) == 0b00000010);
                        runningBits.Add((byte)(b & 0b00000001) == 0b00000001);
                    }
                    else if (b >= 0b10000000){
                        // throw new Exception("Invalid byte");
                    }
                    else {
                        runningBits.Add((byte)(b & 0b10000000) == 0b10000000);
                        runningBits.Add((byte)(b & 0b01000000) == 0b01000000);
                        runningBits.Add((byte)(b & 0b00100000) == 0b00100000);
                        runningBits.Add((byte)(b & 0b00010000) == 0b00010000);
                        runningBits.Add((byte)(b & 0b00001000) == 0b00001000);
                        runningBits.Add((byte)(b & 0b00000100) == 0b00000100);
                        runningBits.Add((byte)(b & 0b00000010) == 0b00000010);
                        runningBits.Add((byte)(b & 0b00000001) == 0b00000001);
                    }
                }
            }

            string utfJoined = string.Join(string.Empty, utf8Characters);
            byte[] utfBytes = utfJoined.Chunk(2).Select(x => Convert.ToByte(new string(x), 16)).ToArray();
            string answer = Encoding.UTF8.GetString(utfBytes);

            return answer;
        }
    }
}