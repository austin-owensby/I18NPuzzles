using System.Text;

namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/08.txt
    public class Solution08Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(8, example);

            int answer = 0;

            List<char> vowels = ['a', 'e', 'i', 'o', 'u'];

            foreach (string line in lines)
            {
                if (line.Length < 4 || line.Length > 12) {
                    continue;
                }

                string normalized = line.Normalize(NormalizationForm.FormD).ToLower();

                if (!normalized.Any(char.IsDigit)) {
                    continue;
                }

                if (!normalized.Any(vowels.Contains)) {
                    continue;
                }

                if (!normalized.Any(c => char.IsLetter(c) && !vowels.Contains(c))) {
                    continue;
                }

                if (normalized.Count(char.IsLetter) != normalized.Distinct().Count(char.IsLetter)) {
                    continue;
                }

                answer++;
            }

            return answer.ToString();
        }
    }
}