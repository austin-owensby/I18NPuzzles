using System.Globalization;
using System.Text;

namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/13.txt
    public class Solution13Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(13, example);
            List<List<string>> data = lines.ChunkByExclusive(string.IsNullOrWhiteSpace);
            List<string> puzzle = data.Last().Select(l => l.Trim()).ToList();
            List<List<string>> puzzleArray = puzzle.Select(l => l.ToTextElementList()).ToList();
            List<byte[]> bytes = data.First().Select(l => l.Chunk(2).Select(c => byte.Parse(new string(c), NumberStyles.HexNumber)).ToArray()).ToList();

            byte[] utf8BOM = new List<string>(){"ef", "bb", "bf"}.Select(c => byte.Parse(c, NumberStyles.HexNumber)).ToArray();
            byte[] utf16beBOM = new List<string>(){"fe", "ff"}.Select(c => byte.Parse(c, NumberStyles.HexNumber)).ToArray();
            byte[] utf16leBOM = new List<string>(){"ff", "fe"}.Select(c => byte.Parse(c, NumberStyles.HexNumber)).ToArray();

            int answer = 0;

            for (int i = 0; i < bytes.Count; i++)
            {
                byte[] line = bytes[i];

                string word;
                if (line[0] == utf8BOM[0] && line[1] == utf8BOM[1] && line[2] == utf8BOM[2]) {
                    word = Encoding.UTF8.GetString(line.Skip(3).ToArray());
                }
                else if (line[0] == utf16beBOM[0] && line[1] == utf16beBOM[1]) {
                    word = Encoding.BigEndianUnicode.GetString(line.Skip(2).ToArray());
                }
                else if (line[0] == utf16leBOM[0] && line[1] == utf16leBOM[1]) {
                    word = Encoding.Unicode.GetString(line.Skip(2).ToArray());
                }
                else if (Encoding.UTF8.GetString(line).All(char.IsLetter)) {
                    word = Encoding.UTF8.GetString(line);
                }
                else if (Encoding.Latin1.GetString(line).All(char.IsLetter)) {
                    word = Encoding.Latin1.GetString(line);
                }
                else if (Encoding.BigEndianUnicode.GetString(line).All(char.IsLetter)) {
                    word = Encoding.BigEndianUnicode.GetString(line);
                }
                else if (Encoding.Unicode.GetString(line).All(char.IsLetter)) {
                    word = Encoding.Unicode.GetString(line);
                }
                else {
                    throw new Exception("Unable to parse into a word");
                }

                List<string> stringArray = word.ToTextElementList().ToList();

                foreach (List<string> puzzleLine in puzzleArray) {
                    if (puzzleLine.Count == stringArray.Count) {
                        int index = puzzleLine.FindIndex(x => x != ".");

                        if (puzzleLine[index] == stringArray[index]) {
                            answer += i + 1;
                            puzzleArray.Remove(puzzleLine);
                            break;
                        }
                    }
                }

                if (puzzleArray.Count == 0) {
                    break;
                }
            }

            return answer.ToString();
        }
    }
}