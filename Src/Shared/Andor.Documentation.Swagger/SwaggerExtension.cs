using Andor.Documentation.Swagger.OperationFilter;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Andor.Documentation.Swagger;

public static class SwaggerExtension
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        var authOptions = builder.Configuration.GetSection("IdentityProvider").Get<IdentityProvider>() ?? new IdentityProvider();

        var scopes = authOptions.Scopes?.ToDictionary(scope => scope);

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{authOptions.Authority}/connect/auth"),
                        TokenUrl = new Uri($"{authOptions.Authority}/connect/token"),
                        Scopes = scopes
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
            options.OperationFilter<SwaggerDefaultValuesFilter>();
        });


        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

        AddApiVersioning(builder.Services);

        return builder;
    }

    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app,
        IApiVersionDescriptionProvider versionDescription,
        IConfiguration configuration)
    {
        var authOptions = configuration.GetSection("IdentityProvider").Get<IdentityProvider>()
            ?? new IdentityProvider();

        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            foreach (var description in
                versionDescription.ApiVersionDescriptions.Select(x => x.GroupName))
            {
                o.SwaggerEndpoint($"/swagger/{description}/swagger.json",
                    $"Andor - {description.ToUpper()}");

                o.RoutePrefix = string.Empty;

                o.OAuthClientId(authOptions.SwaggerClientId ?? "");
                o.OAuthClientSecret(authOptions.SecretKey ?? "");
                o.OAuthAppName("Andor - Swagger");
                o.OAuthUsePkce();
            }
        });

        return app;
    }

    private static void AddApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }
}
