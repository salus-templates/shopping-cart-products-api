// ProductsService/Program.cs

using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ProductService;
using Serilog;
using Newtonsoft.Json;
using ProductService.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpLogging(o => { });
builder.Services.AddPersistence(builder.Configuration);
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
    {
        // Include XML documentation from the API project
        // var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
        // c.IncludeXmlComments(apiXmlPath);
        //
        // // Include XML documentation from the Contracts project
        // var contractsXmlAssembly = Assembly.Load("Database.Contracts");
        // var contractsXmlFile = $"{contractsXmlAssembly.GetName().Name}.xml";
        // var contractsXmlPath = Path.Combine(AppContext.BaseDirectory, contractsXmlFile);
        // c.IncludeXmlComments(contractsXmlPath);
    });
// Configure CORS to allow requests from any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin() // Allow any host to access
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Host.UseSerilog((context, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration) // Read configuration from appsettings.json
            .Enrich.FromLogContext(); // Ensure enrichers are applied
    });

var app = builder.Build();

app.UseHttpLogging();
// Use CORS policy
app.UseCors("AllowAnyOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // UseDeveloperExceptionPage provides detailed error information in development
    app.UseDeveloperExceptionPage();
    // Removed Swagger UI as requested previously
}
app
    .UseSwagger()
    .UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
app.Services.MigrateTransactionsDb();

var foobar = "Unused variable";

// Static list of products (moved from React app)

// Endpoint to get all products
app.MapGet("/all-products", async (ApplicationDbContext dbContext) => await dbContext.Products.ToListAsync())
    .WithName("GetAllProducts");

app.MapPost("/products", async (ApplicationDbContext dbContext, CreateProductRequest createProductRequest, ILogger<Program> logger) =>
    {
        logger.LogInformation("Creating product");
        if (string.IsNullOrEmpty(createProductRequest.Name) || createProductRequest.Price == 0)
        {
            return Results.BadRequest("Please provide a name or a price");
        }

        var product = new Product()
        {
            Name = createProductRequest.Name,
            Description = createProductRequest.Description,
            Price = createProductRequest.Price,
            ImageUrl = createProductRequest.ImageUrl,
            Id = Guid.NewGuid(),
            Stock = createProductRequest.Stock
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Created product");
        return Results.Created($"/all-products/{product.Id}", product);
    })
    .WithName("CreateProduct");


app.MapPost("/place-order", async (ApplicationDbContext dbContext, PlaceOrderRequest orderRequest, ILogger<Program> logger) =>
{
    await Task.Delay(500); // Simulate network delay

    var outOfStockItems = new List<Guid>();

    var products = await dbContext.Products.ToListAsync();
    foreach (var item in orderRequest.Items)
    {
        var productInStock = products.FirstOrDefault(p => p.Id == item.Id);
        if (productInStock == null)
        {
            logger.LogError("Order failed: Product with ID '{ProductId}' not found.", item.Id);
            outOfStockItems.Add(item.Id);
        }
        else if (productInStock.Stock < item.Quantity)
        {
            logger.LogError("Order failed: Product '{ProductName}' (ID: '{ProductId}') is out of stock. Requested: {RequestedQuantity}, Available: {AvailableStock}",
                            productInStock.Name, item.Id, item.Quantity, productInStock.Stock);
            outOfStockItems.Add(item.Id);
        }
    }

    if (outOfStockItems.Any())
    {
        var order = new PlaceOrderResponse
        {
            Success = false,
            Message = "Order failed: Some items are out of stock or requested quantity exceeds available stock.",
            OutOfStockItems = outOfStockItems
        };

        // Return HTTP 400 Bad Request for out-of-stock items
        return Results.BadRequest(order);
    }

    // Simulate stock deduction (in a real app, this would involve database updates)
    foreach (var item in orderRequest.Items)
    {
        var productInStock = products.FirstOrDefault(p => p.Id == item.Id);
        if (productInStock != null) // Should not be null due to prior check, but good for safety
        {
            productInStock.Stock -= item.Quantity;
            logger.LogInformation("Stock updated for '{ProductName}' (ID: '{ProductId}'). New stock: {NewStock}",
                                  productInStock.Name, item.Id, productInStock.Stock);
        }
    }

    // Generate a mock order ID
    var orderId = "ORD" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    // Serialize the order request using Newtonsoft.Json and log it
    var orderJson = JsonConvert.SerializeObject(orderRequest);
    logger.LogInformation("Order placed successfully. Order ID: {OrderId}, Delivery Address: {Address}, Order Details: {OrderJson}",
                         orderId, orderRequest.DeliveryAddress, orderJson);

    return Results.Ok(new PlaceOrderResponse
    {
        Success = true,
        Message = "Order placed successfully!",
        OrderId = orderId
    });

})
.WithName("PlaceOrder");

app.Run("http://0.0.0.0:8088");

public partial class Program { }
