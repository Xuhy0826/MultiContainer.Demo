using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Demo.User.API.SwaggerExt
{
    public class SwaggerAddHeaderParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var version = context.ApiDescription.GroupName;

            //var acceptHeaderText = "application/json;v=" + version.Substring(1);
            operation.Parameters.Add(
                new OpenApiParameter()
                {
                    In = ParameterLocation.Header,
                    Name = "Accept",
                    AllowEmptyValue = true
                });
        }
    }
}
