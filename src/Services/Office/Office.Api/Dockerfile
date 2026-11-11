FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files for caching dotnet restore
COPY ["src/Common/ElPixelClinic.Contracts/ElPixelClinic.Contracts.csproj", "src/Common/ElPixelClinic.Contracts/"]
COPY ["src/Services/Office/Office.Domain/Office.Domain.csproj", "src/Services/Office/Office.Domain/"]
COPY ["src/Services/Office/Office.Application/Office.Application.csproj", "src/Services/Office/Office.Application/"]
COPY ["src/Services/Office/Office.Infrastructure/Office.Infrastructure.csproj", "src/Services/Office/Office.Infrastructure/"]
COPY ["src/Services/Office/Office.Api/Office.Api.csproj", "src/Services/Office/Office.Api/"]

RUN dotnet restore "src/Services/Office/Office.Api/Office.Api.csproj"

# Copy source code and build
COPY ["src/Common/ElPixelClinic.Contracts/", "src/Common/ElPixelClinic.Contracts/"]
COPY ["src/Services/Office/", "src/Services/Office/"]
RUN dotnet build "src/Services/Office/Office.Api/Office.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/Services/Office/Office.Api/Office.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Office.Api.dll"]
