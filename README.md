# Nuredini Cargo API

A small .NET 8 Web API for managing goods, suppliers, warehouses, inventory and stock movements.

Built mostly to explore how I wanted to structure a more enterprise-style .NET backend.

## Stack

- .NET 8 / C#
- Entity Framework Core
- SQL Server
- FluentValidation
- Swagger / OpenAPI
- Repository pattern with async repositories
- Dependency injection
- Unit and integration tests

## Structure

The API is split into controllers, DTOs, entities, validators, repositories and EF Core configuration/migrations.

Nothing revolutionary here. Just a fairly clean backend with enough moving parts to make the interesting problems show up.

## Run

You need a SQL Server instance and a valid connection string in `appsettings.json`.

Then:

```bash
dotnet restore
dotnet run --project NurediniCargoAPI
```

Swagger is available in development mode.

## Tests

```bash
dotnet test
```
