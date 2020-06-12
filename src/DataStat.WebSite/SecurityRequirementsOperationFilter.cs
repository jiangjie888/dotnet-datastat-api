using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DataStat.WebSite
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerPermissions = context.ApiDescription.CustomAttributes()
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Permissions);

            var actionPermissions = context.ApiDescription.ActionDescriptor.()
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Permissions);

            var permissions = controllerPermissions.Union(actionPermissions).Distinct()
                .SelectMany(p => p);

            if (permissions.Any())
            {
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });
                operation.Responses.Add("403", new Response { Description = "Forbidden" });

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        { "bearerAuth", permissions }
                    }
                };
            }
        }
    }
}
