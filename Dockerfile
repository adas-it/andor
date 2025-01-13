#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Andor.Api/Andor.Api.csproj", "src/Andor.Api/"]
COPY ["src/Andor.Kernel/Andor.Ioc.csproj", "src/Andor.Kernel/"]
COPY ["src/Andor.Infrastructure/Andor.Infrastructure.csproj", "src/Andor.Infrastructure/"]
COPY ["src/Andor.Domain/Andor.Domain.csproj", "src/Andor.Domain/"]
COPY ["src/Andor.Application/Andor.Application.csproj", "src/Andor.Application/"]
COPY ["src/Andor.Application.Dto/Andor.Application.Dto.csproj", "src/Andor.Application.Dto/"]
RUN dotnet restore "./src/Andor.Api/Andor.Api.csproj"
COPY . .
WORKDIR "/src/src/Andor.Api"
RUN dotnet build "./Andor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Andor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Andor.Api.dll"]