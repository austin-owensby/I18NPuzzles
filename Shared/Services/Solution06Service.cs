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

            List<List<string>> parts = lines.ChunkByExclusive(string.IsNullOrWhiteSpace);
            List<string> words = parts[0];
            List<string> puzzle = parts[1].Select(w => w.Trim()).ToList();

            List<List<string>> puzzleArray = puzzle.Select(l => l.ToTextElementList()).ToList();

            int answer = 0;

            for (int i = 0; i < words.Count; i++){
                string word = words[i];

                if ((i + 1) % 3 == 0) {
                    byte[] bytes = Encoding.Latin1.GetBytes(word);
                    word = Encoding.UTF8.GetString(bytes);
                }

                if ((i + 1) % 5 == 0) {
                    byte[] bytes = Encoding.Latin1.GetBytes(word);
                    word = Encoding.UTF8.GetString(bytes);
                }

                List<string> stringArray = word.ToTextElementList();

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