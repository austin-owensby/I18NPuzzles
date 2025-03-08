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
            if (parameter.Name.Equals("day", StringComparison.InvariantCultureIgnoreCase))
            {
                List<int> days = Enumerable.Range(1, Globals.NUMBER_OF_PUZZLES).ToList();
                parameter.Schema.Enum = days.Select(d => new OpenApiString(d.ToString())).ToList<IOpenApiAny>();
            }
        }
    }
}