using System.Linq;
using System.Reflection;
using Domain.Core.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Demo.User.API.SwaggerExt
{
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.DeclaringType == null) return;
            var parms = context.MethodInfo.GetParameters();
            foreach (var parameterInfo in parms)
            {
                foreach (var property in parameterInfo.ParameterType.GetProperties())
                {
                    var excludeAttributes = property.GetCustomAttribute<SwaggerExcludeAttribute>();
                    if (excludeAttributes == null) continue;
                    var prop = operation.Parameters.FirstOrDefault(p => p.Name == property.Name);
                    if (prop != null)
                    {
                        operation.Parameters.Remove(prop);
                    }
                }
            }
        }
    }
}
