FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files for caching dotnet restore
COPY ["src/Common/ElPixelClinic.Contracts/ElPixelClinic.Contracts.csproj", "src/Common/ElPixelClinic.Contracts/"]
COPY ["src/Services/Services/Services.Domain/Services.Domain.csproj", "src/Services/Services/Services.Domain/"]
COPY ["src/Services/Services/Services.Application/Services.Application.csproj", "src/Services/Services/Services.Application/"]
COPY ["src/Services/Services/Services.Infrastructure/Services.Infrastructure.csproj", "src/Services/Services/Services.Infrastructure/"]
COPY ["src/Services/Services/Services.Api/Services.Api.csproj", "src/Services/Services/Services.Api/"]

RUN dotnet restore "src/Services/Services/Services.Api/Services.Api.csproj"

# Copy source code and build
COPY ["src/Common/ElPixelClinic.Contracts/", "src/Common/ElPixelClinic.Contracts/"]
COPY ["src/Services/Services/", "src/Services/Services/"]
RUN dotnet build "src/Services/Services/Services.Api/Services.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/Services/Services/Services.Api/Services.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Services.Api.dll"]
