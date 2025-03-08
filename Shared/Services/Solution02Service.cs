namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/02.txt
    public class Solution02Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = Utility.GetInputLines(2, example);
            List<DateTimeOffset> dates = lines.Select(DateTimeOffset.Parse).ToList();
            DateTime detectedWaveTime = dates.GroupBy(l => l.UtcDateTime).OrderByDescending(g => g.Count()).First().Key;

            string answer = detectedWaveTime.ToString("yyyy-MM-ddTHH:mm:sszzz");

            return answer;
        }
    }
}