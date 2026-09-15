var builder = DistributedApplication.CreateBuilder(args);

var configurationsApi = builder.AddProject<Projects.Andor_Configurations_Service>("configurations-service")
    .WithHttpHealthCheck("/health");

var userIdentityApi = builder.AddProject<Projects.Andor_Users_WebApi>("user-identity");

var userServiceApi = builder.AddProject<Projects.Andor_Users_Service>("user-service")
    .WithHttpHealthCheck("/health");

var assetsApi = builder.AddProject<Projects.Andor_Assets_Service>("assets-service")
    .WithHttpHealthCheck("/health");

var accountsApi = builder.AddProject<Projects.Andor_Accounts_Service>("accounts-api")
    .WithHttpHealthCheck("/health");

var communicationsApi = builder.AddProject<Projects.Andor_Communications_Service>("communications-api")
    .WithHttpHealthCheck("/health");

var onboardingApi = builder.AddProject<Projects.Andor_Onboarding_Service>("onboarding-api")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Andor_Admin_ReverseProxy_Yarp>("reverse-proxy", launchProfileName: "https")
    .WithEndpoint("https", endpoint =>
    {
        endpoint.Port = 7000;
        endpoint.IsProxied = false;
    })
    .WithReference(configurationsApi)
    .WithReference(userIdentityApi)
    .WithReference(userServiceApi)
    .WithReference(assetsApi)
    .WithReference(accountsApi)
    .WithReference(communicationsApi)
    .WithReference(onboardingApi)
    .WaitFor(configurationsApi)
    .WaitFor(userIdentityApi)
    .WaitFor(userServiceApi)
    .WaitFor(assetsApi)
    .WaitFor(accountsApi)
    .WaitFor(communicationsApi)
    .WaitFor(onboardingApi);

builder.AddProject<Projects.Andor_Goals_Service>("goals-service")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
