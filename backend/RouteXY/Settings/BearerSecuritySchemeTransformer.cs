using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RouteXY.Api.Settings;

internal sealed class BearerSecuritySchemeTransformer(Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var scheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", 
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token"
            };

            document.Components ??= new OpenApiComponents();
            
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = scheme;
            
            var schemeReference = new OpenApiSecuritySchemeReference("Bearer", document);

            foreach (var path in document.Paths.Values)
            {
                if (path.Operations == null) continue;

                foreach (var operation in path.Operations.Values)
                {
                    operation.Security ??= new List<OpenApiSecurityRequirement>();
                    
                    var requirement = new OpenApiSecurityRequirement
                    {
                        [schemeReference] = new List<string>()
                    };
                    
                    operation.Security.Add(requirement);
                }
            }
        }
    }
}