using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NurediniCargoAPI.Controllers;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories.Interfaces;
using FluentValidation.Results;
using NurediniCargoAPI.Validators;
using NurediniCargoAPI.Repositories;
using System.Net;
using Azure;

namespace NurediniCargoAPI.UnitTests.Controllers
{
    public class GoodsControllerTests
    {

        #region CREATE

        [Fact]
        public async Task AddAsync_WithValidData_ShouldReturnCreatedAtRouteResult()
        {
            // Arrange
            var validGoods = GetSampleGoods();

            var mockRepository = new Mock<IAsyncRepository<Goods>>();
            mockRepository.Setup(repo => repo.AddAsync(validGoods)).ReturnsAsync(validGoods);

            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());

            // Act
            var response = await controller.AddAsync(validGoods);

            // Assert
            response.Result.Should().BeOfType<CreatedAtRouteResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.Created);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<CreatedAtRouteResult>();
            var resultValue = response.Result.As<CreatedAtRouteResult>().Value.Should().BeAssignableTo<Goods>();

            // Assert that the result content is equivalent to the expected validGoods
            resultValue.Subject.Should().BeEquivalentTo(validGoods);
        }

        [Fact]
        public async Task AddAsync_WithInvalidData_ShouldReturnUnprocessableEntityResult()
        {
            // Arrange
            var invalidGoods = GetSampleGoods();

            invalidGoods.Price = 0;

            // Note: Do not mock the validator
            var mockRepository = new Mock<IAsyncRepository<Goods>>();
            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());

            // Act
            var response = await controller.AddAsync(invalidGoods);

            // Assert
            response.Result.Should().BeOfType<UnprocessableEntityObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<UnprocessableEntityObjectResult>();
            var resultValue = response.Result.As<UnprocessableEntityObjectResult>().Value.Should().BeAssignableTo<List<ValidationFailure>>();

            // Assert that the result content is not empty (assuming validation returns a list of ValidationFailure)
            resultValue.Subject.Should().NotBeEmpty();
        }

        #endregion

        #region READ

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfGoods()
        {
            // Arrange
            var goodsList = GetSampleGoodsList();

            var mockRepository = new Mock<IAsyncRepository<Goods>>();
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(goodsList);

            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());

            // Act
            var response = await controller.GetAllAsync();

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<OkObjectResult>();
            var resultValue = response.Result.As<OkObjectResult>().Value.Should().BeAssignableTo<IEnumerable<Goods>>();

            // Assert that the result content is equivalent to the expected goodsList
            resultValue.Subject.Should().BeEquivalentTo(goodsList);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnGoods()
        {
            // Arrange
            var goods = GetSampleGoods();

            var mockRepository = new Mock<IAsyncRepository<Goods>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(goods);

            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());

            // Act
            var response = await controller.GetByIdAsync(1);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultValue = (response.Result as OkObjectResult)?.Value as Goods;
            resultValue.Should().BeEquivalentTo(goods);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var goods = GetSampleGoods();

            var mockRepository = new Mock<IAsyncRepository<Goods>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(goods);

            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());
            var invalidId = 0; // Provide an invalid ID for testing

            // Act
            var response = await controller.GetByIdAsync(invalidId);

            // Assert
            response.Result.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
        #endregion

        #region UPDATE

        [Fact]
        public async Task UpdateAsync_WithValidUpdate_ShouldReturnOkWithUpdatedEntity()
        {
            // Arrange

            // Assume this is an existing entity in the repository
            var existingGoods = GetSampleGoods();

            // Create an updated version of the entity
            var updatedGoods = GetSampleGoods();
            updatedGoods.Description = "High-performance gaming laptop.";
            updatedGoods.Price = 2100.00m;

            // Set up the mock repository with the existing and updated goods
            var mockRepository = new Mock<IAsyncRepository<Goods>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(existingGoods.Id)).ReturnsAsync(existingGoods);
            mockRepository.Setup(repo => repo.UpdateAsync(updatedGoods)).ReturnsAsync(updatedGoods);

            // Assume validation passes for the updat

            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());

            // Act
            var response = await controller.UpdateAsync(existingGoods.Id, updatedGoods);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Ensure that the response content (updated goods) matches the expected updated goods
            var resultValue = (response.Result as OkObjectResult)?.Value as Goods;
            resultValue.Should().BeEquivalentTo(updatedGoods);
        }

        [Fact]
        public async Task UpdateAsync_WithMismatchedIds_ShouldReturnBadRequest()
        {
            // Arrange

            // Assume this is an existing entity in the repository
            var existingGoods = GetSampleGoods();

            // Create an updated version of the entity with a different ID
            var updatedGoods = GetSampleGoods();
            updatedGoods.Id = existingGoods.Id + 1;

            // Set up the mock repository and mock validator
            var mockRepository = new Mock<IAsyncRepository<Goods>>();

            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());

            // Act
            var response = await controller.UpdateAsync(existingGoods.Id, updatedGoods);

            // Assert
            response.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            // Ensure that the response indicates a bad request due to mismatched IDs
            response.Result.As<BadRequestObjectResult>().Value.Should().Be("The ID in the URL does not match the ID in the request body.");
        }

        [Fact]
        public async Task UpdateAsync_WithValidationFailure_ShouldReturnUnprocessableEntityWithValidationErrors()
        {

            // Arrange
            var invalidGoods = GetSampleGoods(); // Define a method to generate invalid goods data

            invalidGoods.UnitOfMeasure = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Note: Do not mock the validator
            var mockRepository = new Mock<IAsyncRepository<Goods>>();
            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());

            // Act
            var response = await controller.UpdateAsync(invalidGoods.Id, invalidGoods);

            // Assert
            response.Result.Should().BeOfType<UnprocessableEntityObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<UnprocessableEntityObjectResult>();
            var resultValue = response.Result.As<UnprocessableEntityObjectResult>().Value.Should().BeAssignableTo<List<ValidationFailure>>();

            // Assert that the result content is not empty (assuming validation returns a list of ValidationFailure)
            resultValue.Subject.Should().NotBeEmpty();
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task DeleteAsync_WithExistingEntity_ShouldReturnNoContent()
        {
            // Arrange
            var existingGoods = GetSampleGoods();

            var mockRepository = new Mock<IAsyncRepository<Goods>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(existingGoods.Id)).ReturnsAsync(existingGoods);

            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());

            // Act
            var response = await controller.DeleteAsync(existingGoods.Id);

            // Assert
            response.Should().BeOfType<NoContentResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // Ensure that the repository's DeleteAsync method was called with the correct entity
            mockRepository.Verify(repo => repo.DeleteAsync(existingGoods), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingEntity_ShouldReturnNotFound()
        {
            // Arrange

            var mockRepository = new Mock<IAsyncRepository<Goods>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(null as Goods);

            var controller = new GoodsController(mockRepository.Object, new GoodsValidator());

            // Act
            var response = await controller.DeleteAsync(1);

            // Assert
            response.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            // Ensure that the repository's DeleteAsync method was not called since the entity is not found
            mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Goods>()), Times.Never);
        }


        #endregion

        private static Goods GetSampleGoods()
        {
            return new()
            {
                Id = 6969,
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
