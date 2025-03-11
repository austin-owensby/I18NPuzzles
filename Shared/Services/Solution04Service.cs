using SharedUtilities;

namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/04.txt
    public class Solution04Service : ISolutionDayService
    {
        private class Trip {
            public DateTimeOffset DepartTime { get; set; }
            public DateTimeOffset ArriveTime { get; set; }
        }

        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(4, example);

            List<Trip> trips = lines.ChunkByExclusive(l => string.IsNullOrWhiteSpace(l)).Select(g => {
                List<string> departData = g[0].QuickRegex("Departure:\\s+([\\w\\/-]+)\\s+(.+)");
                List<string> arriveData = g[1].QuickRegex("Arrival:\\s+([\\w\\/-]+)\\s+(.+)");

                TimeZoneInfo departTimeZone = TimeZoneInfo.FindSystemTimeZoneById(departData[0]);
                TimeZoneInfo arriveTimeZone = TimeZoneInfo.FindSystemTimeZoneById(arriveData[0]);

                DateTime departDate = DateTime.Parse(departData[1]);
                DateTime arriveDate = DateTime.Parse(arriveData[1]);

                return new Trip() {
                    DepartTime = new(departDate, departTimeZone.GetUtcOffset(departDate)),
                    ArriveTime = new(arriveDate, arriveTimeZone.GetUtcOffset(arriveDate))
                };
            }).ToList();

            int answer = 0;

            foreach (Trip trip in trips)
            {
                double minutes = (trip.ArriveTime - trip.DepartTime).TotalMinutes;
                answer += (int)minutes;
            }

            return answer.ToString();
        }
    }
}