const string CorsPolicyName = "AllowConfiguredOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();


var app = builder.Build();

app.UseHttpsRedirection();

// Must run before MapReverseProxy so CORS preflight (OPTIONS) requests are answered here
// instead of being forwarded downstream, and so the actual proxied response gets the
// Access-Control-Allow-Origin header the browser needs.
app.UseCors(CorsPolicyName);

app.MapReverseProxy();

app.Run();
