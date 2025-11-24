# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ProductService/*.csproj ./
RUN dotnet restore

# Copy the remaining source code and build the application
COPY ProductService/. ./
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose port 8080
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ProductService.dll"]
