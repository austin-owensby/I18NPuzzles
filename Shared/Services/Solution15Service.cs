namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../../Inputs/15.txt
    public class Solution15Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = Utility.GetInputLines(15, example);

            int answer = 0;

            foreach (string line in lines)
            {

            }

            return answer.ToString();
        }
    }
}