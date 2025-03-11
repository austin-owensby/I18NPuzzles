using System.Net;
using HtmlAgilityPack;

namespace I18NPuzzles.Gateways
{
    public class I18NPuzzlesGateway
    {
        private HttpClient? client;
        private readonly int throttleInMinutes = 3;
        private DateTimeOffset? lastCall = null;

        /// <summary>
        /// For a given day, get the user's puzzle input
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public async Task<string> ImportInput(int day)
        {
            ThrottleCall();

            HttpRequestMessage message = new(HttpMethod.Get, $"/puzzle/{day}/input");

            if (client == null)
            {
                try
                {
                    InitializeClient();
                }
                catch
                {
                    throw new Exception("Unable to read Cookie.txt. Make sure that it exists in the PuzzleHelper folder. See the ReadMe for more.");
                }
            }

            HttpResponseMessage result = await client!.SendAsync(message);
            string response = await GetSuccessfulResponseContent(result);

            return response;
        }

        /// <summary>
        /// For a given day, get the user's puzzle test input
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public async Task<string> ImportInputExample(int day)
        {
            HttpRequestMessage message = new(HttpMethod.Get, $"/puzzle/{day}/test-input");

            if (client == null)
            {
                try
                {
                    InitializeClient();
                }
                catch
                {
                    throw new Exception("Unable to read Cookie.txt. Make sure that it exists in the PuzzleHelper folder. See the ReadMe for more.");
                }
            }

            HttpResponseMessage result = await client!.SendAsync(message);
            string response = await GetSuccessfulResponseContent(result);

            return response;
        }

        /// <summary>
        /// Send the user's answer to the specific question
        /// </summary>
        /// <param name="day"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public async Task<string> SubmitAnswer(int day, string answer)
        {
            ThrottleCall();

            string? token = await GetCSRFMiddlewareToken(day);

            if (token == null) {
                throw new Exception("Puzzle has already been solved or we couldn't find the token.");
            }

            Dictionary<string, string> data = new()
            {
                { "csrfmiddlewaretoken", token },
                { "answer", answer }
            };

            HttpContent request = new FormUrlEncodedContent(data);

            if (client == null)
            {
                try
                {
                    InitializeClient();
                }
                catch
                {
                    return "Unable to read Cookie.txt. Make sure that it exists in the PuzzleHelper folder. See the ReadMe for more.";
                }
            }

            HttpResponseMessage result = await client!.PostAsync($"/puzzle/{day}/submit", request);

            string response = await GetSuccessfulResponseContent(result);

            try
            {
                // Display the response
                HtmlDocument doc = new();
                doc.LoadHtml(response);
                HtmlNode article = doc.DocumentNode.SelectSingleNode("//b");
                response = article.InnerHtml;
            }
            catch (Exception)
            {
                System.Console.WriteLine("Error parsing html response.");
            }

            return response;
        }

        private async Task<string?> GetCSRFMiddlewareToken(int day) {
            if (client == null)
            {
                try
                {
                    InitializeClient();
                }
                catch
                {
                    return "Unable to read Cookie.txt. Make sure that it exists in the PuzzleHelper folder. See the ReadMe for more.";
                }
            }

            HttpResponseMessage result = await client!.GetAsync($"/puzzle/{day}/");

            string response = await GetSuccessfulResponseContent(result);

            string? token = Utility.QuickRegex(response, "<form.+puzzle\\/\\d+\\/submit.+\\s+.+csrfmiddlewaretoken.+value.+\"(.+)\"").FirstOrDefault();

            return token;
        }

        /// <summary>
        /// Ensure that the response was successful and return the parsed response if it was
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static async Task<string> GetSuccessfulResponseContent(HttpResponseMessage result)
        {
            if (result.StatusCode == HttpStatusCode.Unauthorized) {
                throw new Exception("Your Cookie has expired, please update it. See the ReadMe for more info.");
            }

            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Tracks the last API call and prevents another call from being made until after the configured limit
        /// </summary>
        private void ThrottleCall()
        {
            if (lastCall != null && DateTimeOffset.Now < lastCall.Value.AddMinutes(throttleInMinutes))
            {
                throw new Exception($"Unable to make another API call to I18N Puzzles Server because we are attempting to throttle calls according to their specifications (See more in the ReadMe). Please try again after {lastCall.Value.AddMinutes(throttleInMinutes)}.");
            }
            else
            {
                lastCall = DateTimeOffset.Now;
            }
        }

        /// <summary>
        /// Initialize the Http Client using the user's cookie
        /// </summary>
        private void InitializeClient()
        {
            // We're waiting to do this until the last moment in case someone want's to use the code base without setting up the cookie
            client = new HttpClient
            {
                BaseAddress = new Uri("https://i18n-puzzles.com/")
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd(".NET 8.0 (+via https://github.com/austin-owensby/I18NPuzzles by austin_owensby@hotmail.com)");

            string[] fileData;

            try
            {
                string directoryPath = Directory.GetParent(Environment.CurrentDirectory)!.FullName;
                string filePath = Path.Combine(directoryPath, "Shared", "PuzzleHelper", "Cookie.txt");
                fileData = File.ReadAllLines(filePath);
            }
            catch (Exception)
            {
                throw new Exception("Unable to read Cookie.txt. Make sure that it exists in the PuzzleHelper folder. See the ReadMe for more.");
            }

            if (fileData.Length == 0 || string.IsNullOrWhiteSpace(fileData[0]))
            {
                throw new Exception("Cookie.txt is empty. Please ensure it is properly populated and saved. See the ReadMe for more.");
            }
            if (fileData.Length > 1)
            {
                throw new Exception("Detected multiple lines in Cookie.txt, ensure that the whole cookie is on 1 line.");
            }

            string cookie = fileData[0];
            client.DefaultRequestHeaders.Add("Cookie", cookie);
        }
    }
}