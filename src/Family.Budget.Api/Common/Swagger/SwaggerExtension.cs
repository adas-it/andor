namespace Family.Budget.Api.Swagger;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Family.Budget.Api.Common.Swagger;
using Family.Budget.Api.Common.Swagger.OperationFilter;
using Family.Budget.Application.Common;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetSection("IdentityProvider").Get<IdentityProvider>();

        var scopes = authOptions.Scopes is not null ? authOptions.Scopes.ToDictionary(scope => scope) : null;

        services.AddSwaggerGen(options =>
        {            
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{authOptions.Authority}/protocol/openid-connect/auth"),
                        TokenUrl = new Uri($"{authOptions.Authority}/protocol/openid-connect/token"),
                        Scopes = scopes
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
            options.OperationFilter<SwaggerDefaultValuesFilter>();
        }).AddSwaggerGenNewtonsoftSupport();


        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

        AddApiVersioning(services);

        return services;
    }

    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app,
        IOptions<ApplicationSettings> configuration,
        IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    $"Family.Budget.Api - {description.GroupName.ToUpper()}");

                o.RoutePrefix = string.Empty;

                o.OAuthClientId(configuration.Value.IdentityProvider?.SwaggerClientId);
                o.OAuthClientSecret(configuration.Value.IdentityProvider?.SecretKey);
                o.OAuthAppName("Family.Budget.Api - Swagger");
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
