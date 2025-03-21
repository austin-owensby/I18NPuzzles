namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/15.txt
    public class Solution15Service : ISolutionDayService
    {
        class Office {
            public string Name {get; set;} = string.Empty;
            public TimeZoneInfo TimeZone {get; set;} = TimeZoneInfo.Utc;
            public List<DateTime> Holidays {get; set;} = [];
            public int Overtime {get; set;}

            public Office (string data) {
                List<string> parts = Utility.QuickRegex(data, @"(([\w\s,]+)\t([\w/_-]+)\t(.+))").ToList();

                Name = parts[1];
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(parts[2]);
                Holidays = parts[3].Split(';').Select(DateTime.Parse).ToList();
            }
        }

        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(15, example);
            List<List<string>> inputParts = lines.ChunkByExclusive(string.IsNullOrWhiteSpace);
            List<Office> topLapOffices = inputParts[0].Select(l => new Office(l)).ToList();
            List<Office> customerOffices = inputParts[1].Select(l => new Office(l)).ToList();

            DateTime utcDate = new(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = utcDate.AddYears(1);

            TimeSpan officeStart = new(8, 30, 0);
            TimeSpan officeEnd = new(17, 0, 0);
            
            while (utcDate < end) {
                bool hasCoverage = false;

                foreach (Office office in topLapOffices) {
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDate, office.TimeZone);

                    if (localTime.DayOfWeek == DayOfWeek.Sunday || localTime.DayOfWeek == DayOfWeek.Saturday) {
                        continue;
                    }

                    if (office.Holidays.Contains(localTime.Date)) {
                        continue;
                    }

                    if (localTime.TimeOfDay >= officeStart && localTime.TimeOfDay < officeEnd) {
                        hasCoverage = true;
                        break;
                    }
                }

                if (!hasCoverage) {
                    foreach (Office office in customerOffices) {
                        DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDate, office.TimeZone);

                        if (localTime.DayOfWeek == DayOfWeek.Sunday || localTime.DayOfWeek == DayOfWeek.Saturday) {
                            continue;
                        }

                        if (office.Holidays.Contains(localTime.Date)) {
                            continue;
                        }

                        office.Overtime++;
                    }
                }

                utcDate = utcDate.AddMinutes(1);
            }

            int minOvertime = customerOffices.Min(c => c.Overtime);
            int maxOvertime = customerOffices.Max(c => c.Overtime);
            int answer = maxOvertime - minOvertime;

            return answer.ToString();
        }
    }
}