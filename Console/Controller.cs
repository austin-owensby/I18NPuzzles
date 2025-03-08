using I18NPuzzles.Gateways;
using I18NPuzzles.PuzzleHelper;
using I18NPuzzles.Services;
using Microsoft.Extensions.DependencyInjection;

namespace I18NPuzzles.Console.Controllers
{
    public class Controller {
        private readonly I18NPuzzlesGateway gateway = new();

        /// <summary>
        /// Runs a specific day's solution, and optionally posts the answer to I18N Puzzles and returns the result.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="send">Submit the result to I18N Puzzles</param>
        /// <param name="example">Use an example file instead of the regular input, you must add the example at `Inputs/DD_example.txt`</param>
        public async Task GetSolution(int day = 1, bool send = false, bool example = false) {
            if (send && example)
            {
                System.Console.WriteLine("You're attempting to submit your answer to I18N Puzzles while using an example input, this is likely a mistake.");
            }
            
            SolutionService solutionService = SetupSolutionService();

            string result = await solutionService.GetSolution(day, send, example);
            System.Console.WriteLine(result);
        }

        /// <summary>
        /// Imports the input from I18N Puzzles for a specific day.
        /// </summary>
        /// <remarks>
        /// The program is idempotent (You can run this multiple times as it will only add a file if it is needed.)
        /// </remarks>
        /// <param name="day"></param>
        public async Task ImportInputFile(int day = 1) {
            PuzzleHelperService puzzleHelperService = new(gateway);
            await puzzleHelperService.ImportInputFile(day);
        }

        private SolutionService SetupSolutionService() {
            // Setup access to each daily solution service
            ServiceCollection serviceProvider = new();

            #region Setup Daily Solution Services
            // Get a list of assembly types for the whole app
            IEnumerable<Type> assemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes());

            // Get only the types for the classes that inherit from the ISolutionDayService
            IEnumerable<Type> solutionDayServiceTypes = assemblyTypes.Where(x => !x.IsInterface && x.GetInterface(nameof(ISolutionDayService)) != null);

            // Register each Solution Day Service class
            foreach (Type solutionDayServiceType in solutionDayServiceTypes)
            {
                // This is not null because of the filter a few lines above
                Type interfaceType = solutionDayServiceType.GetInterface(nameof(ISolutionDayService))!;

                serviceProvider.AddTransient(interfaceType, solutionDayServiceType);
            }
            #endregion

            SolutionService solutionService = new(serviceProvider.BuildServiceProvider(), gateway);

            return solutionService;
        }
    }
}