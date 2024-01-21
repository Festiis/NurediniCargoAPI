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
using NurediniCargoAPI.DTOs;

namespace NurediniCargoAPI.UnitTests.Controllers
{
    public class InventoryControllerTests
    {

        #region CREATE

        [Fact]
        public async Task AddAsync_WithValidData_ShouldReturnCreatedAtRouteResult()
        {
            // Arrange
            var validInventoryDTO = GetSampleInventoryDTO();

            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Inventory>()))
                .ReturnsAsync((Inventory addedInventory) => addedInventory);

            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());

            // Act
            var response = await controller.AddAsync(validInventoryDTO);

            // Assert
            response.Result.Should().BeOfType<CreatedAtRouteResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.Created);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<CreatedAtRouteResult>();
            var resultValue = response.Result.As<CreatedAtRouteResult>().Value.Should().BeAssignableTo<Inventory>();

            // Assert that the result content is equivalent to the expected valid inventory
            resultValue.Subject.Should().BeEquivalentTo(validInventoryDTO, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task AddAsync_WithInvalidData_ShouldReturnUnprocessableEntityResult()
        {
            // Arrange
            var invalidInventory = GetSampleInventoryDTO();

            invalidInventory.Quantity = 0;

            // Note: Do not mock the validator
            var mockRepository = new Mock<IInventoryRepository>();
            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());

            // Act
            var response = await controller.AddAsync(invalidInventory);

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
        public async Task GetAllAsync_ShouldReturnListOfInventory()
        {
            // Arrange
            var inventoryList = GetSampleInventoryList();

            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(inventoryList);

            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());

            // Act
            var response = await controller.GetAllAsync();

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<OkObjectResult>();
            var resultValue = response.Result.As<OkObjectResult>().Value.Should().BeAssignableTo<IEnumerable<Inventory>>();

            // Assert that the result content is equivalent to the expected inventoryList
            resultValue.Subject.Should().BeEquivalentTo(inventoryList);
        }


 
        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnInventory()
        {
            // Arrange
            var inventory = GetSampleInventory();

            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(inventory);

            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());

            // Act
            var response = await controller.GetByIdAsync(1);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultValue = (response.Result as OkObjectResult)?.Value as Inventory;
            resultValue.Should().BeEquivalentTo(inventory);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var inventory = GetSampleInventory();

            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(inventory);

            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());
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
            var existingInventory = GetSampleInventory();

            // Create an updated version of the entity
            var updatedInventory = GetSampleInventory();
            updatedInventory.Quantity = 50;

            // Set up the mock repository with the existing and updated inventory
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(existingInventory.Id)).ReturnsAsync(existingInventory);
            mockRepository.Setup(repo => repo.UpdateAsync(updatedInventory)).ReturnsAsync(updatedInventory);

            // Assume validation passes for the update

            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());

            // Act
            var response = await controller.UpdateAsync(existingInventory.Id, updatedInventory);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Ensure that the response content (updated inventory) matches the expected updated inventory
            var resultValue = (response.Result as OkObjectResult)?.Value as Inventory;
            resultValue.Should().BeEquivalentTo(updatedInventory);
        }

        [Fact]
        public async Task UpdateAsync_WithMismatchedIds_ShouldReturnBadRequest()
        {
            // Arrange
            var existingInventory = GetSampleInventory();

            // Create an updated version of the entity with a different ID
            var updatedInventory = GetSampleInventory();
            updatedInventory.Id = existingInventory.Id + 1;

            // Set up the mock repository and mock validator
            var mockRepository = new Mock<IInventoryRepository>();

            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());

            // Act
            var response = await controller.UpdateAsync(existingInventory.Id, updatedInventory);

            // Assert
            response.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            // Ensure that the response indicates a bad request due to mismatched IDs
            response.Result.As<BadRequestObjectResult>().Value.Should().Be("The ID in the URL does not match the ID in the request body.");
        }

        [Fact]
        public async Task UpdateAsync_WithValidationFailure_ShouldReturnUnprocessableEntityWithValidationErrors()
        {

            // Arrange
            var invalidInventory = GetSampleInventory(); // Define a method to generate invalid inventory data

            invalidInventory.Quantity = 0;

            // Note: Do not mock the validator
            var mockRepository = new Mock<IInventoryRepository>();
            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());

            // Act
            var response = await controller.UpdateAsync(invalidInventory.Id, invalidInventory);

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
            var existingInventory = GetSampleInventory();

            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(existingInventory.Id)).ReturnsAsync(existingInventory);


            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());

            // Act
            var response = await controller.DeleteAsync(existingInventory.Id);

            // Assert
            response.Should().BeOfType<NoContentResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // Ensure that the repository's DeleteAsync method was called with the correct entity
            mockRepository.Verify(repo => repo.DeleteAsync(existingInventory), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingEntity_ShouldReturnNotFound()
        {
            // Arrange

            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(null as Inventory);


            var controller = new InventoryController(mockRepository.Object, new InventoryValidator());

            // Act
            var response = await controller.DeleteAsync(1);

            // Assert
            response.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            // Ensure that the repository's DeleteAsync method was not called since the entity is not found
            mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Inventory>()), Times.Never);
        }


        #endregion

        private static Inventory GetSampleInventory()
        {
            return new()
            {
                Id = 6969,
                WarehouseId = 1,
                GoodsId = 101,
                Quantity = 500,
                CreatedBy = "SeedData",
                CreatedDate = DateTime.Now,
                LastModifiedBy = "SeedData",
                LastModifiedDate = DateTime.Now
            };
        }

        private static InventoryDTO GetSampleInventoryDTO()
        {
            return new()
            {
                WarehouseId = 1,
                GoodsId = 101,
                Quantity = 500,
                CreatedBy = "SeedData",
                CreatedDate = DateTime.Now,
                LastModifiedBy = "SeedData",
                LastModifiedDate = DateTime.Now
            };
        }

        private static List<Inventory> GetSampleInventoryList()
        {
            return
        [
            new()
            {
                WarehouseId = 2,
                GoodsId = 102,
                Quantity = 300,
                CreatedBy = "SeedData",
                CreatedDate = DateTime.Now,
                LastModifiedBy = "SeedData",
                LastModifiedDate = DateTime.Now
            },
            new()
            {
                WarehouseId = 3,
                GoodsId = 103,
                Quantity = 1000,
                CreatedBy = "SeedData",
                CreatedDate = DateTime.Now,
                LastModifiedBy = "SeedData",
                LastModifiedDate = DateTime.Now
            }
        ];
        }
    }
}
