namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/19.txt
    public class Solution19Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(19, example);

            // TimeZoneInfo.FindSystemTimeZoneById

            List<(DateTime date, string timeZone)> data = lines.Select(l => l.Split("; ")).Select(l => (DateTime.Parse(l[0]), l[1])).ToList();

            List<string> timeZones = data.Select(d => d.timeZone).Distinct().Order().ToList();

            List<DateTime> allDates = [];

            foreach ((DateTime date, string timeZone) in data)
            {
                // Add original date assuming it's correct
                DateTime utcDate = TimeZoneInfo.ConvertTimeToUtc(date, TimeZoneInfo.FindSystemTimeZoneById(timeZone));
                allDates.Add(utcDate);

                // Add corrected date
                DateTime correctedDate = timeZone switch {
                    "Africa/Casablanca" => utcDate.AddHours(-1),
                    "Africa/Juba" => utcDate.AddHours(-1),
                    "Africa/Sao_Tome" => utcDate.AddHours(1),
                    "America/Mazatlan" => utcDate.AddHours(-1),
                    "America/Mexico_City" => utcDate.AddHours(-1),
                    "America/Santiago" => utcDate.AddHours(-1),
                    "Antarctica/Casey" => utcDate.AddHours(2),
                    "Antarctica/Vostok" => utcDate.AddHours(-2),
                    "Asia/Hebron" => utcDate.AddHours(1), // ?
                    "Asia/Pyongyang" => utcDate.AddHours(0.5),
                    "Asia/Qyzylorda" => utcDate.AddHours(-1),
                    "Asia/Tehran" => utcDate.AddHours(-1),
                    "Europe/Volgograd" => utcDate.AddHours(-1),
                    "Pacific/Easter" => utcDate.AddHours(1), // ?
                    _ => throw new Exception($"Unknown time zone {timeZone}")
                };

                allDates.Add(correctedDate);
            }

            DateTime detectedWaveTime = allDates.GroupBy(l => l).OrderByDescending(g => g.Count()).First().Key;

            string answer = detectedWaveTime.ToString("yyyy-MM-ddTHH:mm:ss+00:00");

            return answer.ToString();
        }
    }
}