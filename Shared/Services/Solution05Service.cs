namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/05.txt
    public class Solution05Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(5, example);
            List<List<string>> grid = lines.Select(l => l.ToTextElementList()).ToList();

            int answer = 0;

            int col = 0;

            foreach (List<string> line in grid)
            {
                if (line[col] == "💩") {
                    answer++;
                }
                
                // Wrap around left to right
                col = (col + 2) % line.Count;
            }

            return answer.ToString();
        }
    }
}