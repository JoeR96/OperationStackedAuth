# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy and restore the main project file
COPY ["./OperationStackedAuth.csproj", "."]
RUN dotnet restore "./OperationStackedAuth.csproj"

# Copy and restore the test project file
COPY ["../OperationStackedAuth.Tests/OperationStackedAuth.Tests.csproj", "../OperationStackedAuth.Tests/"]
RUN dotnet restore "../OperationStackedAuth.Tests/OperationStackedAuth.Tests.csproj"

# Copy the entire solution and build the application
COPY . .
WORKDIR "/src/OperationStackedAuth"
RUN dotnet build "OperationStackedAuth.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "OperationStackedAuth.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image for running the published application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "OperationStackedAuth.dll"]
