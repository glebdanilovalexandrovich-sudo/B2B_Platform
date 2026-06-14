B2B MarketPlace API

Backend for a B2B marketplace built with ASP.Net Core 8, Entity Framework Core, JWT and MS SQL Server.

Technologies:
- C# / .NET 8
- ASP.NET Core Web API
- Entity Framework Core (Code First, migrations, relations)
- MS SQL Server
- JWT + BCrypt
- Swagger / OpenAPI
- Git / GitHub

Features
- JWT authentication with roles (Admin, Supplier, Buyer)
- Products & categories CRUD
- Deal creation with stock validation
- DTOs to hide internal fields
- Swagger documentation

How to run locally:

git clone  https://github.com/glebdanilovalexandrovich-sudo/B2B_Platform
cd B2B_Platform

Update connection string in appsettings.json
Run migrations: dotnet ef database update

Run the project:
dotnet run

Open Swagger: https://localhost:7091/swagger
[B2B.pdf](https://github.com/user-attachments/files/28927575/B2B.pdf)

