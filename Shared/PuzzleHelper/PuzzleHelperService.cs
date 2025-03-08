using I18NPuzzles.Gateways;

namespace I18NPuzzles.PuzzleHelper
{
    public class PuzzleHelperService(I18NPuzzlesGateway i18NPuzzlesGateway)
    {
        private readonly I18NPuzzlesGateway i18NPuzzlesGateway = i18NPuzzlesGateway;

        /// <summary>
        /// Imports the day's input file.
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public async Task<string> ImportInputFile(int day)
        {
            string output = string.Empty;

            int latestPuzzleDay = GetLatestDay();

            if (latestPuzzleDay < day)
            {
                System.Console.WriteLine("No updates applied.");
                output += "No updates applied.\n";
            }
            else
            {
                bool update = await WriteInputFile(day);

                if (update)
                {
                    output = $"Created input file for Day: {day}.";
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
        /// <param name="day"></param>
        /// <returns></returns>
        private async Task<bool> WriteInputFile(int day)
        {
            bool update = false;

            string directoryPath = Directory.GetParent(Environment.CurrentDirectory)!.FullName;
            string folderPath = Path.Combine(directoryPath, "Inputs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string inputFilePath = Path.Combine(directoryPath, "Inputs", $"{day:D2}.txt");

            if (!File.Exists(inputFilePath))
            {
                string response;
                try
                {
                    response = await i18NPuzzlesGateway.ImportInput(day);
                }
                catch (Exception)
                {
                    System.Console.WriteLine("An error occurred while getting the puzzle input from I18N Puzzles");
                    throw;
                }

                using StreamWriter inputFile = new(inputFilePath);
                await inputFile.WriteAsync(response);

                System.Console.WriteLine($"Created input file for Day: {day}.");
                update = true;
            }

            string inputExampleFilePath = Path.Combine(directoryPath, "Inputs", $"{day:D2}_example.txt");

            if (!File.Exists(inputExampleFilePath))
            {
                string response;
                try
                {
                    response = await i18NPuzzlesGateway.ImportInputExample(day);
                }
                catch (Exception)
                {
                    System.Console.WriteLine("An error occurred while getting the puzzle input example from I18N Puzzles");
                    throw;
                }

                using StreamWriter inputExampleFile = new(inputExampleFilePath);
                await inputExampleFile.WriteAsync(response);

                System.Console.WriteLine($"Created input example file for Day: {day}.");
                update = true;
            }

            return update;
        }

        /// <summary>
        /// Based on today's date, calculate the latest I18N Puzzles day available
        /// </summary>
        /// <returns></returns>
        private static int GetLatestDay()
        {
            DateTime now = DateTime.UtcNow.AddHours(Globals.SERVER_UTC_OFFSET).AddDays(-1);
            int latestPuzzleDay;

            // Get the dates that puzzles release on
            List<DateTime> dates = Enumerable.Range(Globals.EVENT_DAY, Globals.NUMBER_OF_PUZZLES).Select(d => new DateTime(Globals.EVENT_YEAR, Globals.EVENT_MONTH, d)).ToList();

            latestPuzzleDay = dates.Count(d => d < now);

            return latestPuzzleDay;
        }
    }
}