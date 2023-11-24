FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Family.Budget.Api/Family.Budget.Api.csproj", "src/Family.Budget.Api/"]
COPY ["src/Family.Budget.Application.Dto/Family.Budget.Application.Dto.csproj", "src/Family.Budget.Application.Dto/"]
COPY ["src/Family.Budget.Kernel/Family.Budget.Kernel.csproj", "src/Family.Budget.Kernel/"]
COPY ["src/Family.Budget.Application/Family.Budget.Application.csproj", "src/Family.Budget.Application/"]
COPY ["src/Family.Budget.Domain/Family.Budget.Domain.csproj", "src/Family.Budget.Domain/"]
COPY ["src/Family.Budget.Infrastructure/Family.Budget.Infrastructure.csproj", "src/Family.Budget.Infrastructure/"]
RUN dotnet restore "src/Family.Budget.Api/Family.Budget.Api.csproj"
COPY . .
WORKDIR "/src/src/Family.Budget.Api"
RUN dotnet build "Family.Budget.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Family.Budget.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Family.Budget.Api.dll"]
