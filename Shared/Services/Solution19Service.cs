namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../../Inputs/19.txt
    public class Solution19Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = Utility.GetInputLines(19, example);

            int answer = 0;

            foreach (string line in lines)
            {

            }

            return answer.ToString();
        }
    }
}