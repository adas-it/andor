FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Andor.Api/Andor.Api.csproj", "src/Andor.Api/"]
COPY ["src/Andor.Application.Dto/Andor.Application.Dto.csproj", "src/Andor.Application.Dto/"]
COPY ["src/Andor.Kernel/Andor.Kernel.csproj", "src/Andor.Kernel/"]
COPY ["src/Andor.Application/Andor.Application.csproj", "src/Andor.Application/"]
COPY ["src/Andor.Domain/Andor.Domain.csproj", "src/Andor.Domain/"]
COPY ["src/Andor.Infrastructure/Andor.Infrastructure.csproj", "src/Andor.Infrastructure/"]
RUN dotnet restore "src/Andor.Api/Andor.Api.csproj"
COPY . .
WORKDIR "/src/src/Andor.Api"
RUN dotnet build "Andor.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Andor.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Andor.Api.dll"]
