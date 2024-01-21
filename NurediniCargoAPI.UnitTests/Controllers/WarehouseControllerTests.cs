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
    public class WarehouseControllerTests
    {

        #region CREATE

        [Fact]
        public async Task AddAsync_WithValidData_ShouldReturnCreatedAtRouteResult()
        {
            // Arrange
            var validWarehouse = GetSampleWarehouse();

            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();
            mockRepository.Setup(repo => repo.AddAsync(validWarehouse)).ReturnsAsync(validWarehouse);

            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());

            // Act
            var response = await controller.AddAsync(validWarehouse);

            // Assert
            response.Result.Should().BeOfType<CreatedAtRouteResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.Created);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<CreatedAtRouteResult>();
            var resultValue = response.Result.As<CreatedAtRouteResult>().Value.Should().BeAssignableTo<Warehouse>();

            // Assert that the result content is equivalent to the expected validWarehouse
            resultValue.Subject.Should().BeEquivalentTo(validWarehouse);
        }

        [Fact]
        public async Task AddAsync_WithInvalidData_ShouldReturnUnprocessableEntityResult()
        {
            // Arrange
            var invalidWarehouse = GetSampleWarehouse();

            invalidWarehouse.Name = "";

            // Note: Do not mock the validator
            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();
            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());

            // Act
            var response = await controller.AddAsync(invalidWarehouse);

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
        public async Task GetAllAsync_ShouldReturnListOfWarehouse()
        {
            // Arrange
            var warehouseList = GetSampleWarehouseList();

            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(warehouseList);

            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());

            // Act
            var response = await controller.GetAllAsync();

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<OkObjectResult>();
            var resultValue = response.Result.As<OkObjectResult>().Value.Should().BeAssignableTo<IEnumerable<Warehouse>>();

            // Assert that the result content is equivalent to the expected warehouseList
            resultValue.Subject.Should().BeEquivalentTo(warehouseList);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnWarehouse()
        {
            // Arrange
            var warehouse = GetSampleWarehouse();

            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(warehouse);

            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());

            // Act
            var response = await controller.GetByIdAsync(1);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultValue = (response.Result as OkObjectResult)?.Value as Warehouse;
            resultValue.Should().BeEquivalentTo(warehouse);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var warehouse = GetSampleWarehouse();

            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(warehouse);

            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());
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
            var existingWarehouse = GetSampleWarehouse();

            // Create an updated version of the entity
            var updatedWarehouse = GetSampleWarehouse();
            updatedWarehouse.Name = "SLC AB";

            // Set up the mock repository with the existing and updated warehouse
            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(existingWarehouse.Id)).ReturnsAsync(existingWarehouse);
            mockRepository.Setup(repo => repo.UpdateAsync(updatedWarehouse)).ReturnsAsync(updatedWarehouse);

            // Assume validation passes for the updat

            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());

            // Act
            var response = await controller.UpdateAsync(existingWarehouse.Id, updatedWarehouse);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Ensure that the response content (updated warehouse) matches the expected updated warehouse
            var resultValue = (response.Result as OkObjectResult)?.Value as Warehouse;
            resultValue.Should().BeEquivalentTo(updatedWarehouse);
        }

        [Fact]
        public async Task UpdateAsync_WithMismatchedIds_ShouldReturnBadRequest()
        {
            // Arrange
            var existingWarehouse = GetSampleWarehouse();

            // Create an updated version of the entity with a different ID
            var updatedWarehouse = GetSampleWarehouse();
            updatedWarehouse.Id = existingWarehouse.Id + 1;

            // Set up the mock repository and mock validator
            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();

            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());

            // Act
            var response = await controller.UpdateAsync(existingWarehouse.Id, updatedWarehouse);

            // Assert
            response.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            // Ensure that the response indicates a bad request due to mismatched IDs
            response.Result.As<BadRequestObjectResult>().Value.Should().Be("The ID in the URL does not match the ID in the request body.");
        }

        [Fact]
        public async Task UpdateAsync_WithValidationFailure_ShouldReturnUnprocessableEntityWithValidationErrors()
        {

            // Arrange
            var invalidWarehouse = GetSampleWarehouse();

            invalidWarehouse.Name = "";

            // Note: Do not mock the validator
            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();
            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());

            // Act
            var response = await controller.UpdateAsync(invalidWarehouse.Id, invalidWarehouse);

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
            var existingWarehouse = GetSampleWarehouse();

            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(existingWarehouse.Id)).ReturnsAsync(existingWarehouse);

            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());

            // Act
            var response = await controller.DeleteAsync(existingWarehouse.Id);

            // Assert
            response.Should().BeOfType<NoContentResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // Ensure that the repository's DeleteAsync method was called with the correct entity
            mockRepository.Verify(repo => repo.DeleteAsync(existingWarehouse), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingEntity_ShouldReturnNotFound()
        {
            // Arrange

            var mockRepository = new Mock<IAsyncRepository<Warehouse>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(null as Warehouse);

            var controller = new WarehouseController(mockRepository.Object, new WarehouseValidator());

            // Act
            var response = await controller.DeleteAsync(1);

            // Assert
            response.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            // Ensure that the repository's DeleteAsync method was not called since the entity is not found
            mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Warehouse>()), Times.Never);
        }


        #endregion

        private static Warehouse GetSampleWarehouse()
        {
            return new()
            {
                Name = "Stockholm Logistics Center",
                Address = "Gustav Adolfs v�g 123",
                City = "Stockholm",
                PostalCode = "12345",
                Country = "Sweden",
                CreatedBy = "SeedData",
                CreatedDate = DateTime.Now,
                LastModifiedBy = "SeedData",
                LastModifiedDate = DateTime.Now
            };
        }

        private static List<Warehouse> GetSampleWarehouseList()
        {
            return
            [
                new()
                {
                    Name = "Gothenburg Distribution Hub",
                    Address = "Karl Johans gata 456",
                    City = "Gothenburg",
                    PostalCode = "54321",
                    Country = "Sweden",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
                new()
                {
                    Name = "Malmo Storage Facility",
                    Address = "S�dra v�gen 789",
                    City = "Malmo",
                    PostalCode = "67890",
                    Country = "Sweden",
                    CreatedBy = "SeedData",
                        CreatedDate = DateTime.Now,
                        LastModifiedBy = "SeedData",
                        LastModifiedDate = DateTime.Now
                }
            ];
        }
    }
}
