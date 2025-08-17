# Multi-stage build for .NET 9 applications
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["KayraExport.Case3.sln", "./"]
COPY ["AuthService/AuthService.csproj", "AuthService/"]
COPY ["ProductService/ProductService.csproj", "ProductService/"]
COPY ["LogService/LogService.csproj", "LogService/"]
COPY ["ApiGateway/ApiGateway.csproj", "ApiGateway/"]
COPY ["Shared/Shared.csproj", "Shared/"]

# Restore dependencies
RUN dotnet restore "KayraExport.Case3.sln"

# Copy source code
COPY . .

# Build the solution
RUN dotnet build "KayraExport.Case3.sln" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "KayraExport.Case3.sln" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory for LogService   
RUN mkdir -p /app/logs

ENTRYPOINT ["dotnet", "ApiGateway.dll"]

