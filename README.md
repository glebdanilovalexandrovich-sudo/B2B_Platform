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
<img width="1898" height="980" alt="image" src="https://github.com/user-attachments/assets/07aaf915-a69a-44b4-af6e-590a5cfa4360" />
<img width="1900" height="738" alt="image" src="https://github.com/user-attachments/assets/fc78e65b-17be-45ac-a4e7-186178965b08" />
<img width="923" height="684" alt="image" src="https://github.com/user-attachments/assets/0bc63f52-5181-4e3a-8f22-eff8aa214dfc" />

