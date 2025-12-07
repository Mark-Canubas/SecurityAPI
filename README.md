# SecurityAPI

ASP.NET Core Web API (.NET 9) with EF Core, SQL Server (LocalDB), and a single static Bearer token for authentication.

- Framework: ASP.NET Core Web API (.NET 9.0)
- Auth: Static Bearer token (custom handler)
- Data: EF Core + SQL Server (LocalDB)

## Architecture

- Controllers
  - Api endpoints
  - Data Annotations for validation
- Models
  - Persistent entities (User)
- DTOs
  - Request/response shapes (RegisterDto, LoginDto, CreateUserDto, UpdateUserDto, UserResponse)

## Security

- Static Bearer token authentication: StaticTokenAuthenticationHandler
  - Header required: Authorization: Bearer <static-token>
  - Not issued per-user; hardcoded in the handler

## Prerequisites

- Visual Studio 2022 with .NET 9 SDK
- SQL Server LocalDB: (localdb)\MSSQLLocalDB
- NuGet packages:
  - Swashbuckle.AspNetCore
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Tools
  - BCrypt.Net-Next
- PMC Installations:
  - Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 9.0.0
  - Install-Package Microsoft.EntityFrameworkCore.Tools -Version 9.0.0
  - Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -Version 9.0.0
  - Install-Package BCrypt.Net-Next -Version 4.0.3

## Configuration

- Connection string: SecurityAPI/appsettings.json
  - ConnectionStrings.DefaultConnection
  
- Program.cs:
  - Adds DbContext with UseSqlServer
  - Registers static token authentication scheme
  - Configures Swagger with Bearer scheme

Static token location:
- SecurityAPI/Services/StaticTokenAuthenticationHandler.cs
  - Update the StaticToken constant with your token value

## Database & Migrations
  - Add-Migration Init
  - Update-Database

## Endpoints

Public:
- POST /auth/register
  - Body: RegisterDto
  - Creates a user with Role = User
    
- POST /auth/login
  - Body: LoginDto
    
Requires Authorization: Bearer <static-token>):
- GET /users
  - Returns { data: UserResponse[] }
- GET /users/{id}
  - Returns UserResponse
- GET /users/by-name/{username}
  - Returns UserResponse
- PATCH /users/user/{id}
  - Body: UpdateUserDto (rehashes password, updates role)
  - Returns updated UserResponse
- DELETE /users/user/{id}
  - Return No Content
