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
    public class SupplierControllerTests 
    {

        #region CREATE
        [Fact]
        public async Task AddAsync_WithValidData_ShouldReturnCreatedAtRouteResult()
        {
            // Arrange
            var validSupplier = GetSampleSupplier();

            var mockRepository = new Mock<IAsyncRepository<Supplier>>();
            mockRepository.Setup(repo => repo.AddAsync(validSupplier)).ReturnsAsync(validSupplier);

            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());

            // Act
            var response = await controller.AddAsync(validSupplier);

            // Assert
            response.Result.Should().BeOfType<CreatedAtRouteResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.Created);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<CreatedAtRouteResult>();
            var resultValue = response.Result.As<CreatedAtRouteResult>().Value.Should().BeAssignableTo<Supplier>();

            // Assert that the result content is equivalent to the expected validSupplier
            resultValue.Subject.Should().BeEquivalentTo(validSupplier);
        }

        [Fact]
        public async Task AddAsync_WithInvalidData_ShouldReturnUnprocessableEntityResult()
        {
            // Arrange
            var invalidSupplier = GetSampleSupplier(); // Define a method to generate invalid supplier data

            invalidSupplier.Email = "abcdefghijklmn";

            // Note: Do not mock the validator
            var mockRepository = new Mock<IAsyncRepository<Supplier>>();
            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());

            // Act
            var response = await controller.AddAsync(invalidSupplier);

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
        public async Task GetAllAsync_ShouldReturnListOfSupplier()
        {
            // Arrange
            var supplierList = GetSampleSupplierList();

            var mockRepository = new Mock<IAsyncRepository<Supplier>>();
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(supplierList);


            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());

            // Act
            var response = await controller.GetAllAsync();

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Use FluentAssertions to assert the content of the response
            response.Result.Should().BeAssignableTo<OkObjectResult>();
            var resultValue = response.Result.As<OkObjectResult>().Value.Should().BeAssignableTo<IEnumerable<Supplier>>();

            // Assert that the result content is equivalent to the expected supplierList
            resultValue.Subject.Should().BeEquivalentTo(supplierList);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnSupplier()
        {
            // Arrange
            var supplier = GetSampleSupplier();

            var mockRepository = new Mock<IAsyncRepository<Supplier>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(supplier);


            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());

            // Act
            var response = await controller.GetByIdAsync(1);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultValue = (response.Result as OkObjectResult)?.Value as Supplier;
            resultValue.Should().BeEquivalentTo(supplier);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var supplier = GetSampleSupplier();

            var mockRepository = new Mock<IAsyncRepository<Supplier>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(supplier);


            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());
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
            var existingSupplier = GetSampleSupplier();

            // Create an updated version of the entity
            var updatedSupplier = GetSampleSupplier();
            updatedSupplier.ContactPerson = "Karl Karlsson";
            updatedSupplier.Email = "karl.karlsson@example.com";

            // Set up the mock repository with the existing and updated supplier
            var mockRepository = new Mock<IAsyncRepository<Supplier>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(existingSupplier.Id)).ReturnsAsync(existingSupplier);
            mockRepository.Setup(repo => repo.UpdateAsync(updatedSupplier)).ReturnsAsync(updatedSupplier);

            // Assume validation passes for the update

            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());

            // Act
            var response = await controller.UpdateAsync(existingSupplier.Id, updatedSupplier);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Ensure that the response content (updated supplier) matches the expected updated supplier
            var resultValue = (response.Result as OkObjectResult)?.Value as Supplier;
            resultValue.Should().BeEquivalentTo(updatedSupplier);
        }

        [Fact]
        public async Task UpdateAsync_WithMismatchedIds_ShouldReturnBadRequest()
        {
            // Arrange
            var existingSupplier = GetSampleSupplier();

            // Create an updated version of the entity with a different ID
            var updatedSupplier = GetSampleSupplier();
            updatedSupplier.Id = existingSupplier.Id + 1;

            // Set up the mock repository and mock validator
            var mockRepository = new Mock<IAsyncRepository<Supplier>>();

            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());

            // Act
            var response = await controller.UpdateAsync(existingSupplier.Id, updatedSupplier);

            // Assert
            response.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            // Ensure that the response indicates a bad request due to mismatched IDs
            response.Result.As<BadRequestObjectResult>().Value.Should().Be("The ID in the URL does not match the ID in the request body.");
        }

        [Fact]
        public async Task UpdateAsync_WithValidationFailure_ShouldReturnUnprocessableEntityWithValidationErrors()
        {

            // Arrange
            var invalidSupplier = GetSampleSupplier();

            invalidSupplier.Email = "karl.karlsson";

            // Note: Do not mock the validator
            var mockRepository = new Mock<IAsyncRepository<Supplier>>();
            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());

            // Act
            var response = await controller.UpdateAsync(invalidSupplier.Id, invalidSupplier);

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
            var existingSupplier = GetSampleSupplier();

            var mockRepository = new Mock<IAsyncRepository<Supplier>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(existingSupplier.Id)).ReturnsAsync(existingSupplier);


            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());

            // Act
            var response = await controller.DeleteAsync(existingSupplier.Id);

            // Assert
            response.Should().BeOfType<NoContentResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            // Ensure that the repository's DeleteAsync method was called with the correct entity
            mockRepository.Verify(repo => repo.DeleteAsync(existingSupplier), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingEntity_ShouldReturnNotFound()
        {
            // Arrange

            var mockRepository = new Mock<IAsyncRepository<Supplier>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(null as Supplier);


            var controller = new SupplierController(mockRepository.Object, new SupplierValidator());

            // Act
            var response = await controller.DeleteAsync(1);

            // Assert
            response.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            // Ensure that the repository's DeleteAsync method was not called since the entity is not found
            mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Supplier>()), Times.Never);
        }


        #endregion

        private static Supplier GetSampleSupplier()
        {
            return new()
            {
                Id = 1,
                Name = "Svenska Leverantör AB",
                ContactPerson = "Anna Svensson",
                Email = "anna.svensson@example.com",
                Phone = "+46701234567",
                Address = "Storgatan 123",
                City = "Stockholm",
                PostalCode = "12345",
                Country = "Sweden",
                CreatedBy = "SeedData",
                CreatedDate = DateTime.Now,
                LastModifiedBy = "SeedData",
                LastModifiedDate = DateTime.Now
            };
        }

        private List<Supplier> GetSampleSupplierList()
        {
            return
            [
                new Supplier
                {
                    Name = "Nordisk Exportföretag",
                    ContactPerson = "Erik Andersson",
                    Email = "erik.andersson@example.com",
                    Phone = "+46769876543",
                    Address = "Lillgatan 456",
                    City = "Göteborg",
                    PostalCode = "54321",
                    Country = "Sweden",
                    CreatedBy = "SeedData",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "SeedData",
                    LastModifiedDate = DateTime.Now
                },
                new Supplier
                {
                    Name = "Östersjö Import AB",
                    ContactPerson = "Maria Johansson",
                    Email = "maria.johansson@example.com",
                    Phone = "+46721112233",
                    Address = "Mellangatan 789",
                    City = "Malmö",
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
