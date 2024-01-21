using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc;
using NurediniCargoAPI.Controllers;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories.Interfaces;
using FluentValidation.Results;
using NurediniCargoAPI.Validators;
using NurediniCargoAPI.Repositories;
using System.Net;
using Azure;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using NurediniCargoAPI.Models.MapsAPI;
using System.Text.Json;

namespace NurediniCargoAPI.IntegrationTests.Controllers
{
    public class GoodsControllerIntegrationTests : IClassFixture<WebApplicationFactory<NurediniCargoAPI.Startup>>
    {

        private readonly WebApplicationFactory<NurediniCargoAPI.Startup> _factory;
        private readonly HttpClient _client;

        public GoodsControllerIntegrationTests(WebApplicationFactory<NurediniCargoAPI.Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }


        #region CREATE

        [Fact]
        public async Task AddAsync_WithValidData_ShouldReturnCreatedAtRouteResult()
        {
            // Arrange
            var validGoods = GetSampleGoods();

            // Act
            var response = await _client.PostAsJsonAsync("/api/goods", validGoods);

            // Assert
            response.Should().NotBeNull("because a response should be received");
            response.StatusCode.Should().Be(HttpStatusCode.Created, "because the request should be successful");

            response.Content.Should().NotBeNull("because the response content should not be null");

            // Deserialize the JSON content into a Goods object
            var createdGoods = await response.Content.ReadFromJsonAsync<Goods>();

            createdGoods.Should().NotBeNull("because the response content should deserialize to a Goods object");

            createdGoods.Id.Should().BeGreaterThan(0, "because a valid entity ID should be greater than 0");
            createdGoods.Name.Should().Be(validGoods.Name, "because the created goods should have the same name as the provided validGoods");

        }

        [Fact]
        public async Task AddAsync_WithInvalidData_ShouldReturnUnprocessableEntityResult()
        {
            // Arrange
            var invalidGoods = GetSampleGoods(); // Define a method to generate invalid goods data

            invalidGoods.Price = 0;

            // Act
            var response = await _client.PostAsJsonAsync("/api/goods", invalidGoods);

            // Assert
            response.Should().NotBeNull("because a response should be received");
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity, "because a valid entity Price should be greater than 0");

        }

        #endregion

        private static Goods GetSampleGoods()
        {
            return new()
            {
                Name = "Laptop",
                Description = "High-performance laptop with the latest technology.",
                Price = 1200.00m,
                MinimumOrderQuantity = 1,
                MaximumOrderQuantity = 10,
                ShippingClass = "Express",
                HandlingInstructions = "Handle with care",
                Brand = "XYZ",
                Category = "Electronics",
                UnitOfMeasure = "Piece",
                CreatedBy = "SeedData",
                CreatedDate = DateTime.Now,
                LastModifiedBy = "SeedData",
                LastModifiedDate = DateTime.Now
            };
        }

        private static List<Goods> GetSampleGoodsList()
        {
            return
            [
                new()
                {
                    Name = "Smart Watch",
                    Description = "Advanced smartwatch with health tracking features.",
                    Price = 199.99m,
                    MinimumOrderQuantity = 10,
                    MaximumOrderQuantity = 50,
                    ShippingClass = "Standard",
                    HandlingInstructions = "Water-resistant",
                    Brand = "PQR",
                    Category = "Wearables",
                    UnitOfMeasure = "Piece",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
                new()
                {
                    Name = "Wireless Headphones",
                    Description = "High-quality wireless headphones with noise-cancellation.",
                    Price = 149.99m,
                    MinimumOrderQuantity = 3,
                    MaximumOrderQuantity = 15,
                    ShippingClass = "Standard",
                    HandlingInstructions = "Store in a cool, dry place",
                    Brand = "DEF",
                    Category = "Audio",
                    UnitOfMeasure = "Pair",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                }
            ];
        }
    }
}
