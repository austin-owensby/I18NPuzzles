using System.Text;

namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/10.txt
    public class Solution10Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(10, example);
            List<List<string>> dbEntries = lines.TakeWhile(l => !string.IsNullOrEmpty(l)).Select(l => l.Split(" ").ToList()).ToList();
            List<List<string>> loginAttempts = lines.SkipWhile(l => !string.IsNullOrEmpty(l)).Skip(1).Select(l => l.Split(" ").ToList()).ToList();

            // Normalize login attempts
            foreach (List<string> loginAttempt in loginAttempts) {
                loginAttempt[1] = loginAttempt[1].Normalize(NormalizationForm.FormD);
            }

            int answer = 0;

            foreach (List<string> dbEntry in dbEntries) {
                string username = dbEntry[0];
                string dbHash = dbEntry[1];
                string salt = dbHash[..29];

                var groups = loginAttempts.Where(x => x[0] == username).GroupBy(x => x[1]);

                // Test each unique login attempt until we find the password
                foreach (var group in groups) {
                    string password = group.Key;
                    List<char> stringArray = password.ToList();
                    bool correct = false;

                    int totalAccents = password.Count(c => !char.IsAscii(c));

                    // Try each combination of the password accent combinations
                    for (int i = 0; i < Math.Pow(2, totalAccents); i++) {
                        string testPassword = string.Empty;
                        int accentIndex = 0;
                        List<bool> accentFlags = Convert.ToString(i, 2).PadLeft(totalAccents).ToList().Select(c => c == '1').ToList();

                        for (int j = 0; j < stringArray.Count; j++) {
                            if (j < stringArray.Count - 1 && !char.IsAscii(stringArray[j + 1])){
                                // Check if this iteration should combine the accent
                                if (accentFlags[accentIndex]) {
                                    string stringToCombine = new([stringArray[j], stringArray[j + 1]]);
                                    string combinedString = stringToCombine.Normalize(NormalizationForm.FormC);
                                    char combinedChar = combinedString[0];
                                    testPassword += combinedChar;

                                    // Skip past the accent on the next iteration
                                    j++;
                                }
                                else {
                                    testPassword += stringArray[j];
                                }

                                accentIndex++;
                            }
                            else {
                                testPassword += stringArray[j];
                            }
                        }

                        correct = dbHash == BCrypt.Net.BCrypt.HashPassword(testPassword, salt);

                        if (correct) {
                            break;
                        }
                    }

                    if (correct) {
                        answer += group.Count();
                        break;
                    }
                }
            }

            return answer.ToString();
        }
    }
}