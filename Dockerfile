# Use the .NET 8 SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ["shopping-cart-backend.sln", "./"]
COPY ["ProductService/ProductService.csproj", "ProductService/"]
COPY ["ProductService.Tests/ProductService.Tests.csproj", "ProductService.Tests/"]

# Restore dependencies
RUN dotnet restore "shopping-cart-backend.sln"

# Copy the rest of the source code
COPY . .

# Build and publish the ProductService
WORKDIR "/src/ProductService"
RUN dotnet publish "ProductService.csproj" -c Release -o /app/publish

# Use the ASP.NET 8 runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProductService.dll"]
