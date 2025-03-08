using System.Text;

namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/01.txt
    public class Solution01Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = Utility.GetInputLines(1, example);

            int answer = 0;

            int byteLimit = 160;
            int charLimit = 140;

            foreach (string line in lines)
            {
                int byteCount = Encoding.UTF8.GetByteCount(line);
                int charCount = line.Length;

                if (byteCount <= byteLimit && charCount <= charLimit) {
                    answer += 13;
                }
                else if (byteCount <= byteLimit) {
                    answer += 11;
                }
                else if (charCount <= charLimit) {
                    answer += 7;
                }
            }

            return answer.ToString();
        }
    }
}