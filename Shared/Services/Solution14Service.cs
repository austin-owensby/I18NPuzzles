namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/14.txt
    public class Solution14Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(14, example);

            double answer = 0;

            double baseUnit = 10.0/33.0;

            Dictionary<string, long> digitMap = new() {
                {"一", 1},
                {"二", 2},
                {"三", 3},
                {"四", 4},
                {"五", 5},
                {"六", 6},
                {"七", 7},
                {"八", 8},
                {"九", 9},
                {"十", 10},
                {"百", 100},
                {"千", 1000},
                {"万", 10000},
                {"億", 100000000}
            };

            Dictionary<string, double> unitMap = new(){
                {"毛", baseUnit / 10000.0},
                {"厘", baseUnit / 1000.0},
                {"分", baseUnit / 100.0},
                {"寸", baseUnit / 10.0},
                {"尺", baseUnit},
                {"間", baseUnit * 6.0},
                {"丈", baseUnit * 10.0},
                {"町", baseUnit * 360.0},
                {"里", baseUnit * 12960.0}
            };

            foreach (string line in lines)
            {
                List<List<string>> parts = line.Split(" × ").Select(p => p.ToTextElementList()).ToList();

                List<string> valueText1 = parts[0].Take(parts[0].Count - 1).ToList();
                List<long> value1Parts = valueText1.Select(x => digitMap[x]).ToList();
                long value1 = 0;

                List<List<long>> myriads1 = value1Parts.ChunkByInclusive(i => i == 10000 || i == 100000000);

                foreach (List<long> myriad in myriads1) {
                    if (myriad.Count == 1) {
                        value1 += myriad[0];
                    }
                    else {
                        long power = myriad.Last();

                        if (power == 10000 || power == 100000000) {
                            value1 += power * myriad.Take(myriad.Count - 1).ChunkByInclusive(c => c % 10 == 0).Sum(c => c.Aggregate((a, b) => a * b));
                        }
                        else {
                            value1 += myriad.ChunkByInclusive(c => c % 10 == 0).Sum(c => c.Aggregate((a, b) => a * b));
                        }
                    }
                }

                string unitText1 = parts[0].Last();
                double unitValue1 = unitMap[unitText1];

                List<string> valueText2 = parts[1].Take(parts[1].Count - 1).ToList();
                List<long> value2Parts = valueText2.Select(x => digitMap[x]).ToList();
                long value2 = 0;

                List<List<long>> myriads2 = value2Parts.ChunkByInclusive(i => i == 10000 || i == 100000000);

                foreach (List<long> myriad in myriads2) {
                    if (myriad.Count == 1) {
                        value2 += myriad[0];
                    }
                    else {
                        long power = myriad.Last();

                        if (power == 10000 || power == 100000000) {
                            value2 += power * myriad.Take(myriad.Count - 1).ChunkByInclusive(c => c % 10 == 0).Sum(c => c.Aggregate((a, b) => a * b));
                        }
                        else {
                            value2 += myriad.ChunkByInclusive(c => c % 10 == 0).Sum(c => c.Aggregate((a, b) => a * b));
                        }
                    }
                }

                string unitText2 = parts[1].Last();
                double unitValue2 = unitMap[unitText2];

                double value = value1 * unitValue1 * value2 * unitValue2;
                answer += value;
            }

            return answer.ToString();
        }
    }
}