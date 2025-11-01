using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using ProductService;

namespace ProductService.Tests;

public class OrderPlacementTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrderPlacementTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task PlaceOrder_ValidOrder_ReturnsSuccess()
    {
        var orderRequest = new PlaceOrderRequest
        {
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { Id = Guid.NewGuid(), Quantity = 1 }
            },
            TotalAmount = 99.99m,
            DeliveryAddress = "123 Test Street",
            OrderDate = DateTime.UtcNow.ToString()
        };

        var json = JsonSerializer.Serialize(orderRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/place-order", content);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<PlaceOrderResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(orderResponse);
        Assert.True(orderResponse.Success);
        Assert.Equal("Order placed successfully!", orderResponse.Message);
        Assert.False(string.IsNullOrEmpty(orderResponse.OrderId));
        Assert.StartsWith("ORD", orderResponse.OrderId);
    }

    [Fact]
    public async Task PlaceOrder_ProductNotFound_ReturnsBadRequest()
    {
        var productId = Guid.NewGuid();
        var orderRequest = new PlaceOrderRequest
        {
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { Id = productId, Quantity = 1 }
            },
            TotalAmount = 99.99m,
            DeliveryAddress = "123 Test Street",
            OrderDate = DateTime.UtcNow.ToString()
        };

        var json = JsonSerializer.Serialize(orderRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/place-order", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<PlaceOrderResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(orderResponse);
        Assert.False(orderResponse.Success);
        Assert.Contains("out of stock", orderResponse.Message);
        Assert.Contains(productId, orderResponse.OutOfStockItems);
    }

    [Fact]
    public async Task PlaceOrder_InsufficientStock_ReturnsBadRequest()
    {
        var productId = Guid.NewGuid();
        var orderRequest = new PlaceOrderRequest
        {
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { Id = productId, Quantity = 1000 } // Much more than available
            },
            TotalAmount = 99.99m,
            DeliveryAddress = "123 Test Street",
            OrderDate = DateTime.UtcNow.ToString()
        };

        var json = JsonSerializer.Serialize(orderRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/place-order", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<PlaceOrderResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(orderResponse);
        Assert.False(orderResponse.Success);
        Assert.Contains("out of stock", orderResponse.Message);
        Assert.Contains(productId, orderResponse.OutOfStockItems);
    }

    [Fact]
    public async Task PlaceOrder_MultipleItems_ValidOrder_ReturnsSuccess()
    {
        var orderRequest = new PlaceOrderRequest
        {
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { Id = Guid.NewGuid(), Quantity = 1 },
                new OrderItemRequest { Id = Guid.NewGuid(), Quantity = 1 }
            },
            TotalAmount = 299.98m,
            DeliveryAddress = "456 Another Street",
            OrderDate = DateTime.UtcNow.ToString()
        };

        var json = JsonSerializer.Serialize(orderRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/place-order", content);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<PlaceOrderResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(orderResponse);
        Assert.True(orderResponse.Success);
        Assert.False(string.IsNullOrEmpty(orderResponse.OrderId));
    }

    [Fact]
    public async Task PlaceOrder_EmptyItems_ReturnsSuccess()
    {
        var orderRequest = new PlaceOrderRequest
        {
            Items = new List<OrderItemRequest>(),
            TotalAmount = 0m,
            DeliveryAddress = "789 Empty Order Street",
            OrderDate = DateTime.UtcNow.ToString()
        };

        var json = JsonSerializer.Serialize(orderRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/place-order", content);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<PlaceOrderResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(orderResponse);
        Assert.True(orderResponse.Success);
    }
}