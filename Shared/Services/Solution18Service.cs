namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/18.txt
    public class Solution18Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(18, example);

            long answer = 0;

            foreach (string line in lines)
            {
                string original = new (line.Where(char.IsAscii).ToArray());
                long result = (long)Math.Round(GetResult(original));

                string fixedValue = GetEquation(line, false);
                long fixedResult = (long)Math.Round(GetResult(fixedValue));

                long difference = Math.Abs(result - fixedResult);
                answer += difference;
            }

            return answer.ToString();
        }

        private static double GetResult(string data) {
            string outerEquation = string.Empty;
            string innerEquation = string.Empty;
            int indent = 0;

            foreach (char c in data) {
                if (indent != 0) {
                    if (!(indent == 1 && c == ')')) {
                        innerEquation += c;
                    }
                }

                if (c == '(') {
                    indent++;
                }
                
                if (indent == 0) {
                    outerEquation += c;
                }
                
                if (c == ')') {
                    indent--;

                    if (indent == 0) {
                        outerEquation += GetResult(innerEquation).ToString();
                        innerEquation = string.Empty;
                    }
                }
            }

            string[] parts = outerEquation.Split(' ');
            double num1 = double.Parse(parts[0]);
            char op = parts[1][0];
            double num2 = double.Parse(parts[2]);

            double result = op switch {
                '+' => num1 + num2,
                '-' => num1 - num2,
                '*' => num1 * num2,
                '/' => num1 / num2,
                _ => throw new NotImplementedException($"Unexpected operator '{op}'")
            };

            return result;
        }
    
        private static string GetEquation(string data, bool flip) {
            string outerEquation = string.Empty;
            string innerEquation = string.Empty;

            int embeddingLevel = 0;

            foreach (char c in data) {
                if (embeddingLevel != 0) {
                    if (!(embeddingLevel == 1 && c == '⁩')) {
                        innerEquation += c;
                    }
                }

                if (c == '⁧' || c == '⁦') {
                    embeddingLevel++;
                }
                
                if (embeddingLevel == 0 && char.IsAscii(c)) {
                    outerEquation += c;
                }
                
                if (c == '⁩') {
                    embeddingLevel--;

                    if (embeddingLevel == 0) {
                        outerEquation += GetEquation(innerEquation, true);
                        innerEquation = string.Empty;
                    }
                }
            }

            // Flip equation
            if (flip) {
                outerEquation = FlipEquation(outerEquation);
            }

            return outerEquation;
        }
    
        private static string FlipEquation(string data) {
            string flipped = string.Empty;

            string currentNumber = string.Empty;

            for (int i = data.Length - 1; i >= 0; i--) {
                if (char.IsDigit(data[i])) {
                    currentNumber = data[i] + currentNumber;
                }
                else {
                    if (!string.IsNullOrEmpty(currentNumber)) {
                        flipped += currentNumber;
                    }

                    if (data[i] == '(') {
                        flipped += ')';
                    }
                    else if (data[i] == ')') {
                        flipped += '(';
                    }
                    else {
                        flipped += data[i];
                    }

                    currentNumber = string.Empty;
                }
            }

            return flipped;
        }
    }
}