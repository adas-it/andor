# syntax=docker/dockerfile:1
#
# Single reusable image definition for every deployable Andor service.
# Pick the service with build args:
#
#   docker build \
#     --build-arg PROJECT=Src/Budget/Andor.Accounts.Service/Andor.Accounts.Service.csproj \
#     --build-arg APP_DLL=Andor.Accounts.Service.dll \
#     -t andor/accounts-api .
#
# Service            PROJECT                                                                          APP_DLL
# -----------------  ------------------------------------------------------------------------------    ---------------------------------
# configurations     Src/Administrations/Configurations/Andor.Configurations.Service/Andor.Configurations.Service.csproj   Andor.Configurations.Service.dll
# users-api          Src/Administrations/Users/Andor.Users.WebApi/Andor.Users.WebApi.csproj                                Andor.Users.WebApi.dll
# assets-service     Src/Assets/Andor.Assets.Service/Andor.Assets.Service.csproj                                          Andor.Assets.Service.dll
# accounts-api       Src/Budget/Andor.Accounts.Service/Andor.Accounts.Service.csproj                                      Andor.Accounts.Service.dll
# communications-api Src/Communications/Andor.Communications.Service/Andor.Communications.Service.csproj                   Andor.Communications.Service.dll
# onboarding-api     Src/Onboarding/Andor.Onboarding.Service/Andor.Onboarding.Service.csproj                              Andor.Onboarding.Service.dll
# goals-service      Src/Goals/Andor.Goals.Service/Andor.Goals.Service.csproj                                            Andor.Goals.Service.dll
# reverse-proxy      Src/Administrations/ReverseProxy/Andor.Admin.ReverseProxy.Yarp/Andor.Admin.ReverseProxy.Yarp.csproj  Andor.Admin.ReverseProxy.Yarp.dll

ARG DOTNET_VERSION=10.0

# ---------------------------------------------------------------------------
# Build / publish
# ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG PROJECT
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Central Package Management + Directory.Build.props must be present before restore.
COPY Directory.Build.props Directory.Packages.props Andor.slnx ./
COPY Src/ Src/

RUN --mount=type=cache,id=andor-nuget,target=/root/.nuget/packages \
    dotnet restore "${PROJECT}"

RUN --mount=type=cache,id=andor-nuget,target=/root/.nuget/packages \
    dotnet publish "${PROJECT}" \
        -c "${BUILD_CONFIGURATION}" \
        --no-restore \
        -o /app/publish \
        /p:UseAppHost=false

# ---------------------------------------------------------------------------
# Runtime
# ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
ARG APP_DLL
WORKDIR /app

# Kestrel listens on 8080 (plain HTTP); TLS is terminated by the platform ingress.
ENV ASPNETCORE_HTTP_PORTS=8080 \
    APP_DLL=${APP_DLL}

COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080

# APP_DLL is resolved from the environment at container start.
ENTRYPOINT ["sh", "-c", "exec dotnet \"$APP_DLL\" \"$@\"", "--"]
