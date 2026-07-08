var builder = DistributedApplication.CreateBuilder(args);

var usersApi = builder.AddProject<Projects.Andor_Users_WebApi>("users-api");

var assetsApi = builder.AddProject<Projects.Andor_Assets_Service>("assets-service")
    .WithHttpHealthCheck("/health");

var accountsApi = builder.AddProject<Projects.Andor_Accounts_WebApi>("accounts-api")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Andor_Admin_ReverseProxy_Yarp>("reverse-proxy")
    .WithReference(usersApi)
    .WithReference(assetsApi)
    .WithReference(accountsApi)
    .WaitFor(usersApi)
    .WaitFor(assetsApi)
    .WaitFor(accountsApi);

builder.Build().Run();
