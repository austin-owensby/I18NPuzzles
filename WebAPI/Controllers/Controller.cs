using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using I18NPuzzles.PuzzleHelper;
using I18NPuzzles.Services;

namespace I18NPuzzles.WebAPI.Controllers
{
    /// <summary>
    /// Base Controller for the Web API
    /// </summary>
    /// <param name="solutionService"></param>
    /// <param name="puzzleHelperService"></param>
    [ApiController]
    [Route("api")]
    public class Controller(SolutionService solutionService, PuzzleHelperService puzzleHelperService) : ControllerBase
    {
        private readonly SolutionService solutionService = solutionService;
        private readonly PuzzleHelperService puzzleHelperService = puzzleHelperService;

        /// <summary>
        /// Runs a specific day's solution, and optionally posts the answer to I18N Puzzles and returns the result.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="send">Submit the result to I18N Puzzles</param>
        /// <param name="example">Use an example file instead of the regular input, you must add the example at `Inputs/DD_example.txt`</param>
        /// <response code="200">The result of running the solution. If submitting the solution, also returns the response from I18N Puzzles.</response>
        [HttpGet("run-solution")]
        public async Task<ActionResult<string>> GetSolution([FromQuery, BindRequired] int day = 1, bool send = false, bool example = false)
        {
            if (send && example)
            {
                return BadRequest("You're attempting to submit your answer to I18N Puzzles while using an example input, this is likely a mistake.");
            }

            try
            {
                return await solutionService.GetSolution(day, send, example);
            }
            catch (SolutionNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Imports the input from I18N Puzzles for a specific day.
        /// </summary>
        /// <remarks>
        /// The program is idempotent (You can run this multiple times as it will only add a file if it is needed.)
        /// </remarks>
        /// <param name="day"></param>
        /// <response code="200">A message on what was updated.</response>
        [HttpPost("import-input-file")]
        public async Task<string> ImportInputFile([FromQuery, BindRequired] int day = 1)
        {
            return await puzzleHelperService.ImportInputFile(day);
        }
    }
}