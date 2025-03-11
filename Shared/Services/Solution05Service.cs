using System.Globalization;

namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/05.txt
    public class Solution05Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(5, example);

            int answer = 0;

            int col = 0;

            // Find the true line length considering emojis
            int lineLength = 0;
            TextElementEnumerator iterator = StringInfo.GetTextElementEnumerator(lines[0]);
            while (iterator.MoveNext()) {
                lineLength++;
            }

            foreach (string line in lines)
            {
                // Iterate over text elements instead of just characters to account for emojis
                iterator = StringInfo.GetTextElementEnumerator(line);

                int index = 0;

                while (iterator.MoveNext()) {
                    if (index == col && iterator.GetTextElement() == "💩") {
                        answer++;
                    }
                    index++;

                    if (index > col) {
                        break;
                    }
                }
                
                // Wrap around left to right
                col = (col + 2) % lineLength;
            }

            return answer.ToString();
        }
    }
}