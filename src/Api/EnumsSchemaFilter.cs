using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AgendamentoAcademia.API.Api
{
    public sealed class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum) return;

            schema.Type = "string";
            schema.Format = null;
            schema.Enum = Enum.GetNames(context.Type).Select(s => (IOpenApiAny)new OpenApiString(s)).ToList();
        }
    }
}
