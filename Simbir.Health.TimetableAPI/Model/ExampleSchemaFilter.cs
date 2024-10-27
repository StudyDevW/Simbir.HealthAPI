using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Simbir.Health.TimetableAPI.Model.Database.DTO;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Simbir.Health.TimetableAPI.Model
{
    public class ExampleSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(Appointments_Write))
            {
                schema.Properties["time"].Example = new OpenApiString("2024-01-01T00:00:00Z");
            }


            if (context.Type == typeof(Timetable_Create))
            {
                schema.Properties["from"].Example = new OpenApiString("2024-01-01T00:00:00Z");
                schema.Properties["to"].Example = new OpenApiString("2024-01-01T12:00:00Z");
            }
        }
    }
}
