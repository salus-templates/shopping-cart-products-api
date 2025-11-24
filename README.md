# shopping-cart-products-api

The Products API, written in Dotnet, is part of the Shopping Cart experience. It provides the product catalog as a Restful API.

## Deployment Instructions

The application is deployable
- As a dotnet app on supported platform. with `ProductService` specified as the project.
- As a webapp leveraging the provided Dockerfile.

### Port

The application is exposed on port `8080`.

## Environment Variables

- `ConnectionStrings__ApplicationDbConnection` - database connection string e.g. `User ID=shoppingcartadmin;Password=mysecurepassowrd;Server=127.0.0.1;Port=5432;Database=shoppingcart;Pooling=true;`
- `ASPNETCORE_URLS` - (optional) if present, overrides the bind address for the application e.g. `0.0.0.0:8080`
