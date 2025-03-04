using I18NPuzzles.Gateways;

namespace I18NPuzzles.PuzzleHelper
{
    public class PuzzleHelperService(I18NPuzzlesGateway i18NPuzzlesGateway)
    {
        private readonly I18NPuzzlesGateway i18NPuzzlesGateway = i18NPuzzlesGateway;

        /// <summary>
        /// Generates solution files.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GenerateMissingSolutionServiceFiles()
        {
            string output = string.Empty;

            bool update = false;

            string directoryPath = Directory.GetParent(Environment.CurrentDirectory)!.FullName;

            // Create a folder for each year that is missing one
            DateTime now = DateTime.UtcNow.AddHours(Globals.SERVER_UTC_OFFSET);
            for (int year = Globals.START_YEAR; year <= now.Year; year++)
            {
                string yearFolderPath = Path.Combine(directoryPath, "Shared", "Services", year.ToString());

                if (!Directory.Exists(yearFolderPath))
                {
                    Directory.CreateDirectory(yearFolderPath);
                    System.Console.WriteLine($"Created folder for {year}.");
                    output += $"Created folder for {year}.\n";
                    update = true;
                }

                // Create/update files for each day that is missing one
                for (int day = 1; day <= Globals.NUMBER_OF_PUZZLES; day++)
                {
                    string dayFilePath = Path.Combine(yearFolderPath, $"Solution{year}_{day:D2}Service.cs");

                    if (!File.Exists(dayFilePath))
                    {
                        // Initialize the new service file
                        using StreamWriter serviceFile = new(dayFilePath);

                        await serviceFile.WriteAsync($$"""
            namespace I18NPuzzles.Services
            {
                // (ctrl/command + click) the link to open the input file
                // file://./../../../Inputs/{{year}}/{{day:D2}}.txt
                public class Solution{{year}}_{{day:D2}}Service : ISolutionDayService
                {
                    public string RunSolution(bool example)
                    {
                        List<string> lines = Utility.GetInputLines({{year}}, {{day}}, example);

                        int answer = 0;

                        foreach (string line in lines)
                        {

                        }

                        return answer.ToString();
                    }
                }
            }
            """);

                        System.Console.WriteLine($"Created solution file for Year: {year}, Day: {day}.");
                        output += $"Created solution file for Year: {year}, Day: {day}.\n";
                        update = true;
                    }
                }
            }

            if (!update)
            {
                System.Console.WriteLine("No updates applied.");
                output += "No updates applied.\n";
            }

            return output;
        }

        /// <summary>
        /// Imports the day's input file.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public async Task<string> ImportInputFile(int year, int day)
        {
            string output = string.Empty;

            Tuple<int, int> latestResults = GetLatestYearAndDate();
            int latestPuzzleYear = latestResults.Item1;
            int latestPuzzleDay = latestResults.Item2;

            if (latestPuzzleYear < year || latestPuzzleYear == year && latestPuzzleDay < day)
            {
                System.Console.WriteLine("No updates applied.");
                output += "No updates applied.\n";
            }
            else
            {
                bool update = await WriteInputFile(year, day);

                if (update)
                {
                    output = $"Created input file for Year: {year}, Day: {day}.";
                }
                else
                {
                    System.Console.WriteLine("No updates applied.");
                    output += "No updates applied.\n ";
                }
            }

            return output;
        }

        /// <summary>
        /// Fetch and write the input file if it doesn't exist
        /// </summary>
        /// <param name="year"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        private async Task<bool> WriteInputFile(int year, int day)
        {
            bool update = false;

            string directoryPath = Directory.GetParent(Environment.CurrentDirectory)!.FullName;
            string yearFolderPath = Path.Combine(directoryPath, $"Inputs/{year}");

            if (!Directory.Exists(yearFolderPath))
            {
                Directory.CreateDirectory(yearFolderPath);
            }

            string inputFilePath = Path.Combine(directoryPath, "Inputs", year.ToString(), $"{day:D2}.txt");

            if (!File.Exists(inputFilePath))
            {
                string response;
                try
                {
                    response = await i18NPuzzlesGateway.ImportInput(year, day);
                }
                catch (Exception)
                {
                    System.Console.WriteLine("An error occurred while getting the puzzle input from I18N Puzzles");
                    throw;
                }

                using StreamWriter inputFile = new(inputFilePath);
                await inputFile.WriteAsync(response);

                System.Console.WriteLine($"Created input file for Year: {year}, Day: {day}.");
                update = true;
            }

            return update;
        }

        /// <summary>
        /// Based on today's date, calculate the latest I18N Puzzles year and day available
        /// </summary>
        /// <returns></returns>
        private static Tuple<int, int> GetLatestYearAndDate()
        {
            DateTime now = DateTime.UtcNow.AddHours(Globals.SERVER_UTC_OFFSET).AddDays(-1);
            int latestPuzzleYear, latestPuzzleDay;

            // This calculation is left as an exercise to the reader
            // TODO, this code assumes that the first puzzle releases on the first Friday of March, need to confirm this.
            List<DateTime> dates = Enumerable.Range(1, Globals.NUMBER_OF_PUZZLES).Select(d => new DateTime(now.Year, Globals.EVENT_MONTH, d).AddDays((7 + (int)DayOfWeek.Friday - (int)new DateTime(now.Year, Globals.EVENT_MONTH, 1).DayOfWeek) % 7)).ToList();

            if (now < dates[0]) {
                // The event has not started yet
                latestPuzzleYear = now.Year - 1;
                latestPuzzleDay = Globals.NUMBER_OF_PUZZLES;
            }
            else {
                latestPuzzleYear = now.Year;
                latestPuzzleDay = dates.Count(d => d < now);
            }

            return Tuple.Create(latestPuzzleYear, latestPuzzleDay);
        }
    }
}