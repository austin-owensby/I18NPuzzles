using System.Globalization;
using System.Text;

namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/06.txt
    public class Solution06Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(6, example);

            // TODO could be a utility function
            List<string> words = lines.TakeWhile(l => !string.IsNullOrWhiteSpace(l)).ToList();
            List<string> puzzle = lines.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1).Select(l => l.Trim()).ToList();

            // TODO could be a utility function
            List<List<string>> puzzleArray = [];
            foreach (string puzzleLine in puzzle) {
                List<string> puzzleItem = [];
                TextElementEnumerator iterator = StringInfo.GetTextElementEnumerator(puzzleLine);
                while (iterator.MoveNext()) {
                    puzzleItem.Add(iterator.GetTextElement());
                }

                puzzleArray.Add(puzzleItem);
            }

            int answer = 0;

            for (int i = 0; i < lines.Count; i++){
                string line = lines[i];

                if ((i + 1) % 3 == 0) {
                    byte[] bytes = Encoding.Latin1.GetBytes(line);
                    line = Encoding.UTF8.GetString(bytes);
                }

                if ((i + 1) % 5 == 0) {
                    byte[] bytes = Encoding.Latin1.GetBytes(line);
                    line = Encoding.UTF8.GetString(bytes);
                }

                List<string> stringArray = [];

                TextElementEnumerator iterator = StringInfo.GetTextElementEnumerator(line);
                while (iterator.MoveNext()) {
                    stringArray.Add(iterator.GetTextElement());
                }

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