# Nuredini Cargo API

Nuredini Cargo API is a .NET Core Web API application. It allows users to manage goods, suppliers, warehouses, and inventory.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Issues, Features, and Should-Haves](#issues-features-and-should-haves)

## Features

- **Goods Management:** Create, update, and delete goods, along with their associated information such as price, quantity, and suppliers.

- **Supplier Management:** Track suppliers and their details, including contact information and location.

- **Warehouse Management:** Manage warehouses, including their name, address, city, postal code, and country.

- **Inventory Tracking:** Keep track of inventory levels in different warehouses, supporting efficient logistics operations.

- **Swagger Documentation:** Utilize Swagger/OpenAPI for clear and interactive API documentation.

- **Entity Framework Core:** Leverage Entity Framework Core for database operations, ensuring a robust and efficient data access layer.

## Technologies Used

- [.NET Core](https://dotnet.microsoft.com/): Cross-platform, high-performance framework for building modern, cloud-based, and internet-connected applications.

- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/): Lightweight, extensible, and cross-platform Object-Relational Mapping (ORM) framework.

- [Fluent Validation](https://fluentvalidation.net/): A popular .NET library for building strongly-typed validation rules.

- [Swagger/OpenAPI](https://swagger.io/): API documentation and visualization tool that enhances the development workflow.

- [xUnit/NUnit](https://xunit.net/): Testing frameworks for unit and integration testing.

## Prerequisites

This project assumes you have SQL Sever installed on your machine:

- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Getting Started

Build the project run it. Explore the API endpoints documented in Swagger to understand how to interact with the Nuredini Cargo API effectively.

## Issues, Features, and Should-Haves

[🚀 Implement Address Validation](https://github.com/Festiis/NurediniCargoAPI/issues/3)

[🐛🚀 Inventory Updates with Movement CRUD Operations](https://github.com/Festiis/NurediniCargoAPI/issues/1)

[🚀🔨 Enhance Test Coverage](https://github.com/Festiis/NurediniCargoAPI/issues/4)

[🔨 Implement BaseController for common CRUD Operations](https://github.com/Festiis/NurediniCargoAPI/issues/2)

[🔨 Implement Data Validation Middleware](https://github.com/Festiis/NurediniCargoAPI/issues/5)
