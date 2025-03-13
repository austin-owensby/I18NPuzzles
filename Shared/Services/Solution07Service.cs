namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/07.txt
    public class Solution07Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(7, example);

            int answer = 0;

            TimeZoneInfo halifaxTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Halifax");
            TimeZoneInfo santiagoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Santiago");

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                // Parse the data
                List<string> data = line.QuickRegex("(.+)\\s+(\\d+)\\s+(\\d+)");
                DateTimeOffset date = DateTimeOffset.Parse(data[0]);
                int correctMinutes = int.Parse(data[1]);
                int incorrectMinutes = int.Parse(data[2]);

                // Determine the correct timezone by comparing the offset in minutes
                TimeSpan halifaxUTCOffset = halifaxTimeZone.GetUtcOffset(date);
                TimeZoneInfo correctTimeZone = halifaxUTCOffset.TotalMinutes == date.TotalOffsetMinutes ? halifaxTimeZone : santiagoTimeZone;

                // Adjust the time based on the audit logs
                DateTimeOffset utcDate = date.UtcDateTime;
                utcDate = utcDate.AddMinutes(-incorrectMinutes);
                utcDate = utcDate.AddMinutes(correctMinutes);

                // Convert the time into the correct timezone
                DateTimeOffset correctedDate = TimeZoneInfo.ConvertTime(utcDate, correctTimeZone);

                answer += correctedDate.Hour * (i + 1);
            }

            return answer.ToString();
        }
    }
}