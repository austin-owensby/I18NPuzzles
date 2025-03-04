using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace I18NPuzzles.Controllers
{
    /// <summary>
    /// Configures parameter filters
    /// </summary>
    public class ParameterFilter : IParameterFilter
    {
        /// <summary>
        /// Applies parameter filters to the API calls in Swagger
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            // Ensure that the input day is a valid value (1 - 20)
            // This does not check if the currently selected year has that date
            if (parameter.Name.Equals("day", StringComparison.InvariantCultureIgnoreCase))
            {
                List<int> days = Enumerable.Range(1, Globals.NUMBER_OF_PUZZLES).ToList();
                parameter.Schema.Enum = days.Select(d => new OpenApiString(d.ToString())).ToList<IOpenApiAny>();
            }

            // Ensure that the input year is a valid value (2025 - this year)
            if (parameter.Name.Equals("year", StringComparison.InvariantCultureIgnoreCase))
            {
                DateTime now = DateTime.UtcNow.AddHours(Globals.SERVER_UTC_OFFSET);

                List<int> days = Enumerable.Range(Globals.START_YEAR, now.Year - Globals.START_YEAR + 1).ToList();
                parameter.Schema.Enum = days.Select(d => new OpenApiString(d.ToString())).ToList<IOpenApiAny>();
            }
        }
    }
}