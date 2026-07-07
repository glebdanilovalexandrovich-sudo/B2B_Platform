![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)
![Docker](https://img.shields.io/badge/Docker-✔-blue)
![License](https://img.shields.io/badge/License-MIT-green)

B2B Marketplace API

B2B Marketplace API — A RESTful backend for managing deals, products, and user roles.

Contact: (I want to get job)
Email - glebdanilovalexandrovich@gmail.com
Telegram - @Rushpile

Technologies

- C# / .NET 8
- ASP.NET Core Web API
- Entity Framework Core (Code First, migrations, relationships)
- MS SQL Server
- JWT + BCrypt
- Swagger / OpenAPI
- REST API
- Git / GitHub
- Docker
- Middleware



Features

- JWT authentication with roles: `Admin`, `Supplier`, `Buyer`
- Role-based access control (`[Authorize(Roles = "...")]`)
- CRUD for products and categories
- Deal management: create, confirm, reject, cancel
- Stock validation and rollback on deal cancellation/rejection
- Transactions with `RepeatableRead` isolation level
- DTOs for secure data transfer
- Swagger documentation
- Docker support
- Middleware for checking errors

---

Project Structure

OptPlatform.sln
├── OptPlatform.Domain // Entities (Product, User, Deal, etc.)
├── OptPlatform.Application // DTOs
├── OptPlatform.Infrastructure // DbContext, Migrations
└── OptPlatform.Api // Controllers, Program.cs

---

How install

Run with Docker (recommended):
git clone https://github.com/glebdanilovalexandrovich-sudo/B2B_Platform.git
cd B2B_Platform
docker-compose up -d --build
Swagger: https://localhost:7091/swagger
Stop containers:
docker-compose down

Nothing docker:
1. git clone https://github.com/glebdanilovalexandrovich-sudo/B2B_Platform.git
   cd B2B_Platform
2. Open
3. In appsettings.json (inside OptPlatform.Api project), update:
"ConnectionStrings": {
    "DefaultConnection": "Server=HOME-PC\\SQLEXPRESS;Database=EF_Core_Tech_quest;Trusted_Connection=True;Encrypt=False;" (If you use a different SQL Server (e.g. localhost or (localdb)), change Server=...)
}
4. Install .NET 8 SDK
5. Run in terminal / Package Manager Console: dotnet restore
6. Apply migrations (create DB): dotnet ef database update --project OptPlatform.Infrastructure --startup-project OptPlatform.Api
7. Run the project: dotnet run --project OptPlatform.Api
8.Open Swagger: https://localhost:7091/swagger

<img width="1893" height="948" alt="image" src="https://github.com/user-attachments/assets/e493c067-fd2d-4bfe-b910-8fe5e47a6cd4" />
<img width="1897" height="821" alt="image" src="https://github.com/user-attachments/assets/1f6f18c3-229b-4061-83cb-f8df506c83b9" />
<img width="802" height="671" alt="image" src="https://github.com/user-attachments/assets/97d634d8-093a-4202-aec0-cfb0c32e1572" />




