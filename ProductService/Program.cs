// ProductsService/Program.cs

using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ProductService;
using Serilog;
using Newtonsoft.Json;
using ProductService.Persistence;
using System.Diagnostics;

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

// Add comprehensive startup logging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("=== Application Starting Up ===");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("Application Name: {ApplicationName}", app.Environment.ApplicationName);
logger.LogWarning("This is a warning log for testing log aggregation");
logger.LogDebug("Debug information: Application configuration loaded successfully");

app.UseHttpLogging();

// Add request logging middleware
app.Use(async (context, next) =>
{
    var requestLogger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    
    requestLogger.LogInformation("[REQUEST START] {Method} {Path} from {RemoteIpAddress}", 
        context.Request.Method, 
        context.Request.Path,
        context.Connection.RemoteIpAddress);
    
    await next.Invoke();
    
    stopwatch.Stop();
    requestLogger.LogInformation("[REQUEST END] {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
        context.Request.Method,
        context.Request.Path, 
        context.Response.StatusCode,
        stopwatch.ElapsedMilliseconds);
    
    if (stopwatch.ElapsedMilliseconds > 1000)
    {
        requestLogger.LogWarning("[PERFORMANCE] Slow request detected: {Method} {Path} took {ElapsedMilliseconds}ms",
            context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
    }
});

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
app.MapGet("/all-products", async (ApplicationDbContext dbContext, ILogger<Program> logger) => 
{
    logger.LogInformation("[GET_ALL_PRODUCTS] Fetching all products from database");
    
    try 
    {
        var products = await dbContext.Products.ToListAsync();
        logger.LogInformation("[GET_ALL_PRODUCTS] Successfully retrieved {ProductCount} products", products.Count);
        
        if (products.Count == 0)
        {
            logger.LogWarning("[GET_ALL_PRODUCTS] No products found in database");
        }
        else
        {
            logger.LogDebug("[GET_ALL_PRODUCTS] Product details: {ProductNames}", 
                string.Join(", ", products.Select(p => p.Name)));
        }
        
        return Results.Ok(products);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[GET_ALL_PRODUCTS] Error retrieving products from database");
        return Results.Problem("Internal server error while fetching products");
    }
})
.WithName("GetAllProducts");

app.MapPost("/products", async (ApplicationDbContext dbContext, CreateProductRequest createProductRequest, ILogger<Program> logger) =>
    {
        logger.LogInformation("[CREATE_PRODUCT] Starting product creation process");
        logger.LogDebug("[CREATE_PRODUCT] Request payload: Name={Name}, Price={Price}, Stock={Stock}", 
            createProductRequest.Name, createProductRequest.Price, createProductRequest.Stock);
        
        // Validation logging
        if (string.IsNullOrEmpty(createProductRequest.Name))
        {
            logger.LogWarning("[CREATE_PRODUCT] Validation failed: Product name is required");
            return Results.BadRequest("Please provide a product name");
        }
        
        if (createProductRequest.Price <= 0)
        {
            logger.LogWarning("[CREATE_PRODUCT] Validation failed: Price must be greater than 0, received {Price}", createProductRequest.Price);
            return Results.BadRequest("Please provide a valid price greater than 0");
        }
        
        if (createProductRequest.Stock < 0)
        {
            logger.LogWarning("[CREATE_PRODUCT] Validation failed: Stock cannot be negative, received {Stock}", createProductRequest.Stock);
            return Results.BadRequest("Stock cannot be negative");
        }

        try
        {
            var product = new Product()
            {
                Name = createProductRequest.Name,
                Description = createProductRequest.Description,
                Price = createProductRequest.Price,
                ImageUrl = createProductRequest.ImageUrl,
                Id = Guid.NewGuid(),
                Stock = createProductRequest.Stock
            };
            
            logger.LogInformation("[CREATE_PRODUCT] Generated product ID: {ProductId} for product: {ProductName}", 
                product.Id, product.Name);

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
            
            logger.LogInformation("[CREATE_PRODUCT] Successfully created product: {ProductName} (ID: {ProductId}) with price ${Price} and stock {Stock}", 
                product.Name, product.Id, product.Price, product.Stock);
                
            // Log metrics for monitoring
            logger.LogInformation("[METRICS] Product created - Name: {ProductName}, Category: Electronics, Price: {Price}",
                product.Name, product.Price);
                
            return Results.Created($"/all-products/{product.Id}", product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[CREATE_PRODUCT] Error creating product: {ProductName}", createProductRequest.Name);
            return Results.Problem("Internal server error while creating product");
        }
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

// Add test endpoints for log aggregation testing
app.MapGet("/test/logs", (ILogger<Program> logger) =>
{
    logger.LogTrace("[TEST] Trace level log - very detailed information");
    logger.LogDebug("[TEST] Debug level log - debugging information");
    logger.LogInformation("[TEST] Info level log - general information");
    logger.LogWarning("[TEST] Warning level log - something unexpected happened");
    logger.LogError("[TEST] Error level log - an error occurred but app continues");
    logger.LogCritical("[TEST] Critical level log - serious failure");
    
    // Test structured logging with different data types
    logger.LogInformation("[TEST] Structured log with data: UserId={UserId}, Action={Action}, Duration={Duration}ms, Success={Success}",
        12345, "TestAction", 250, true);
        
    logger.LogInformation("[TEST] Complex object logging: {@Order}", new { 
        OrderId = "TEST-001", 
        Items = new[] { "Item1", "Item2" }, 
        Total = 99.99m,
        Timestamp = DateTime.UtcNow 
    });
    
    return Results.Ok(new { message = "Test logs generated successfully", timestamp = DateTime.UtcNow });
})
.WithName("GenerateTestLogs");

app.MapGet("/test/error", (ILogger<Program> logger) =>
{
    logger.LogWarning("[TEST_ERROR] Intentionally triggering error for testing");
    try
    {
        // Simulate various types of errors
        var random = new Random();
        var errorType = random.Next(1, 4);
        
        switch (errorType)
        {
            case 1:
                throw new InvalidOperationException("Simulated invalid operation error");
            case 2:
                throw new ArgumentNullException("testParam", "Simulated null argument error");
            case 3:
                throw new TimeoutException("Simulated timeout error");
            default:
                throw new Exception("Generic simulated error");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[TEST_ERROR] Caught exception in test error endpoint: {ErrorMessage}", ex.Message);
        return Results.Problem(detail: "Simulated error for testing", statusCode: 500);
    }
})
.WithName("TestError");

app.MapPost("/test/bulk-logs", (ILogger<Program> logger, BulkLogRequest request) =>
{
    logger.LogInformation("[BULK_LOG] Starting bulk log generation: {Count} logs requested", request.Count);
    
    var logLevels = new[] { "Information", "Warning", "Error", "Debug" };
    var actions = new[] { "UserLogin", "DataProcessing", "FileUpload", "EmailSent", "PaymentProcessed" };
    var random = new Random();
    
    for (int i = 1; i <= Math.Min(request.Count, 100); i++) // Limit to 100 for safety
    {
        var level = logLevels[random.Next(logLevels.Length)];
        var action = actions[random.Next(actions.Length)];
        var userId = random.Next(1000, 9999);
        var duration = random.Next(50, 2000);
        
        switch (level)
        {
            case "Information":
                logger.LogInformation("[BULK_LOG_{Index}] {Action} completed for User {UserId} in {Duration}ms", 
                    i, action, userId, duration);
                break;
            case "Warning":
                logger.LogWarning("[BULK_LOG_{Index}] {Action} completed with warnings for User {UserId}", 
                    i, action, userId);
                break;
            case "Error":
                logger.LogError("[BULK_LOG_{Index}] {Action} failed for User {UserId} - Error: {ErrorCode}", 
                    i, action, userId, random.Next(400, 599));
                break;
            case "Debug":
                logger.LogDebug("[BULK_LOG_{Index}] Debug info for {Action} - User {UserId}, Memory: {Memory}KB", 
                    i, action, userId, random.Next(1024, 8192));
                break;
        }
        
        // Add some delay to simulate realistic timing
        if (i % 10 == 0)
        {
            await Task.Delay(10);
        }
    }
    
    logger.LogInformation("[BULK_LOG] Completed bulk log generation: {Count} logs created", Math.Min(request.Count, 100));
    return Results.Ok(new { message = $"Generated {Math.Min(request.Count, 100)} test logs", timestamp = DateTime.UtcNow });
})
.WithName("GenerateBulkLogs");

// Add periodic health check logging
var timer = new System.Timers.Timer(30000); // 30 seconds
timer.Elapsed += (sender, e) => {
    logger.LogInformation("[HEALTH_CHECK] Application is running normally at {Timestamp}", DateTime.UtcNow);
    logger.LogDebug("[METRICS] Memory usage: {WorkingSet} bytes", GC.GetTotalMemory(false));
};
timer.Start();

// Log application startup completion
logger.LogInformation("=== Application Started Successfully ===");
logger.LogInformation("Server listening on http://0.0.0.0:8080");
logger.LogInformation("Swagger UI available at http://0.0.0.0:8080/swagger");

// Add shutdown hook for cleanup logging
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) => {
    logger.LogWarning("=== Application Shutdown Initiated ===");
    timer?.Stop();
    timer?.Dispose();
    cts.Cancel();
    e.Cancel = true;
};

try
{
    await app.RunAsync("http://0.0.0.0:8080", cts.Token);
}
catch (OperationCanceledException)
{
    logger.LogInformation("=== Application Shutdown Complete ===");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }

// Request models for test endpoints
public record BulkLogRequest(int Count);
