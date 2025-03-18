namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/11.txt
    public class Solution11Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(11, example);
            lines = lines.Select(l => l.ToUpper()).ToList();

            int answer = 0;

            List<string> matches = ["Οδυσσευς", "Οδυσσεως", "Οδυσσει", "Οδυσσεα", "Οδυσσευ"];
            matches = matches.Select(c => c.ToUpper()).ToList();

            foreach (string line in lines)
            {
                string newLine = line;
                for (int i = 0; i < 26; i++) {
                    // Check for matches
                    if (matches.Any(newLine.Contains)) {
                        answer += i;
                        break;
                    }

                    // Shift the sentence
                    List<char> newLineChars = [];

                    for (int j = 0; j < newLine.Length; j++) {
                        char c = newLine[j];
                        if (!char.IsLetter(c)) {
                            newLineChars.Add(c);
                        }
                        else if (c == 'Ρ') {
                            newLineChars.Add('Σ');
                        }
                        else if (c == 'Ω') {
                            newLineChars.Add('Α');
                        }
                        else {
                            newLineChars.Add((char)(c + 1));
                        }
                    }

                    newLine = new (newLineChars.ToArray());
                }
            }

            return answer.ToString();
        }
    }
}