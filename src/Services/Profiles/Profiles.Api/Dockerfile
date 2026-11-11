FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files for caching dotnet restore
COPY ["src/Common/ElPixelClinic.Contracts/ElPixelClinic.Contracts.csproj", "src/Common/ElPixelClinic.Contracts/"]
COPY ["src/Services/Profiles/Profiles.Domain/Profiles.Domain.csproj", "src/Services/Profiles/Profiles.Domain/"]
COPY ["src/Services/Profiles/Profiles.Application/Profiles.Application.csproj", "src/Services/Profiles/Profiles.Application/"]
COPY ["src/Services/Profiles/Profiles.Infrastructure/Profiles.Infrastructure.csproj", "src/Services/Profiles/Profiles.Infrastructure/"]
COPY ["src/Services/Profiles/Profiles.Api/Profiles.Api.csproj", "src/Services/Profiles/Profiles.Api/"]

RUN dotnet restore "src/Services/Profiles/Profiles.Api/Profiles.Api.csproj"

# Copy source code and build
COPY ["src/Common/ElPixelClinic.Contracts/", "src/Common/ElPixelClinic.Contracts/"]
COPY ["src/Services/Profiles/", "src/Services/Profiles/"]
RUN dotnet build "src/Services/Profiles/Profiles.Api/Profiles.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/Services/Profiles/Profiles.Api/Profiles.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Profiles.Api.dll"]
