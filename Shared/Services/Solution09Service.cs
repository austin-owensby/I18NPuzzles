namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/09.txt
    public class Solution09Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(9, example);

            // Parse the data
            var data = lines.Select(x => x.Split(": ")).Select(x => new{
                DateString = x[0],
                Date = x[0].Split("-").ToInts(),
                Names = x[1].Split(", ")
            });

            List<string> answer = [];

            // Loop over each name to determine their format and if it falls on September 11th 2001
            List<string> names = data.SelectMany(d => d.Names).Distinct().Order().ToList();
            foreach (string name in names)
            {
                List<string> possibleFormats = ["DMY", "MDY", "YMD", "YDM"];

                var entries = data.Where(d => d.Names.Contains(name)).ToList();

                // Eliminate formats that can't parse entries in the remaining available formats until we have just 1 valid format remaining
                foreach (var entry in entries) {
                    if (possibleFormats.Count == 1) {
                        break;
                    }

                    if (possibleFormats.Contains("DMY") && !DateOnly.TryParseExact(entry.DateString, "dd-MM-yy", out DateOnly dateParsed))
                    {
                        possibleFormats.Remove("DMY");
                    }

                    if (possibleFormats.Contains("MDY") && !DateOnly.TryParseExact(entry.DateString, "MM-dd-yy", out dateParsed)) {
                        possibleFormats.Remove("MDY");
                    }

                    if (possibleFormats.Contains("YMD") && !DateOnly.TryParseExact(entry.DateString, "yy-MM-dd", out dateParsed)) {
                        possibleFormats.Remove("YMD");
                    }

                    if (possibleFormats.Contains("YDM") && !DateOnly.TryParseExact(entry.DateString, "yy-dd-MM", out dateParsed)) {
                        possibleFormats.Remove("YDM");
                    }
                }

                // Now that we know the format of the date, see if any can be parsed to September 11th 2001
                foreach (var entry in entries) {
                    DateOnly date =  possibleFormats.First() switch {
                        "DMY" => DateOnly.ParseExact(entry.DateString, "dd-MM-yy"),
                        "MDY" => DateOnly.ParseExact(entry.DateString, "MM-dd-yy"),
                        "YMD" => DateOnly.ParseExact(entry.DateString, "yy-MM-dd"),
                        "YDM" => DateOnly.ParseExact(entry.DateString, "yy-dd-MM"),
                        _ => throw new Exception($"Unexpected format: {possibleFormats.First()}")
                    };

                    if (date.Year == 2001 && date.Month == 9 && date.Day == 11) {
                        answer.Add(name);
                    }
                }
            }

            return string.Join(" ", answer);
        }
    }
}