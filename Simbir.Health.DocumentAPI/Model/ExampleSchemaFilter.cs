using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Simbir.Health.DocumentAPI.Model.Database.DTO;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Simbir.Health.DocumentAPI.Model
{
    public class ExampleSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(History_Create))
            {
                schema.Properties["date"].Example = new OpenApiString("2024-01-01T00:00:00Z");
            }

        }
    }
}
