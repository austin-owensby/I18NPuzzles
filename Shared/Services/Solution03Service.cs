namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/03.txt
    public class Solution03Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(3, example);

            int answer = 0;

            foreach (string line in lines)
            {
                if (line.Length < 4 || line.Length > 12) {
                    continue;
                }

                if (!line.Any(c => char.IsDigit(c))) {
                    continue;
                }

                if (!line.Any(c => char.IsUpper(c))) {
                    continue;
                }

                if (!line.Any(c => char.IsLower(c))) {
                    continue;
                }

                if (!line.Any(c => !char.IsAscii(c))) {
                    continue;
                }

                answer++;
            }

            return answer.ToString();
        }
    }
}