using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SortEnumValuesByNameSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear(); // Clear existing enum values

            // Get enum values sorted by name
            var enumValues = Enum.GetNames(context.Type)
                                  .OrderBy(name => name)
                                  .Select(name => Enum.Parse(context.Type, name))
                                  .ToArray();

            // Add sorted enum values to the schema
            foreach (var enumValue in enumValues)
            {
                schema.Enum.Add(new OpenApiString(Enum.GetName(context.Type, enumValue)));
            }
        }
    }
}