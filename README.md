# WPFW_1

Made with HTML, CSS and JavaScript. The backend uses ASP.NET Core and SQL Server.

## Start the backend

You need **Windows**, the **.NET 10 SDK** and **SQL Server Database Engine**.

### 1. Set up SQL Server

During installation, choose:

- **Database Engine Services** to install the database.
- **Default instance** (`MSSQLSERVER`).
- **Windows Authentication** to use your Windows account.
- **Add Current User** to give your account access.

Open Windows **Services** and check that **SQL Server (MSSQLSERVER)** is running.

### 2. Check the database connection

Open `backend/appsettings.json`. The default connection is:

```text
Server=localhost;Database=WpfwPortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;
```

Keep credentials in the ignored `backend/appsettings.Local.json` file or use the
`ConnectionStrings__PortfolioDatabase` environment variable.

### 3. Run the backend and apply migrations

Run these commands from the repository root.

```powershell
dotnet restore Portfolio.slnx
dotnet run --project backend
```

In Development, startup applies the EF Core migrations and adds sample data to
empty portfolio tables. The Identity migration adds the account and role tables
while preserving existing projects and blog posts.

To apply the migrations separately, install the EF Core CLI if needed and run
the database update from the repository root.

```powershell
dotnet tool install --global dotnet-ef --version 10.0.9
dotnet ef database update --project backend
```

If the portfolio tables predate EF migrations, run
`backend/Data/BaselineExistingDatabase.sql` against that existing database before
the update. It records the original migration after checking the existing schema.

### 4. Register, sign in, and assign roles

Open [Swagger](http://localhost:5074/swagger/index.html). Use **Try it out** on
`POST /api/auth/register` and enter your own development email and password.
A successful registration returns `201` and assigns `Gast`. Use the same
credentials on `POST /api/auth/login`. A successful login returns `204` and
sets the `Portfolio.Auth` cookie. The browser sends it on subsequent requests
to the same origin.

Register a separate account for each role you need. With the API stopped,
run the following commands for existing accounts in the local development database.

```powershell
dotnet run --project backend -- --assign-role editor@example.test Editor
dotnet run --project backend -- --assign-role admin@example.test Admin
```

Replace the example emails with the accounts you registered. Restart the API and
log in again after changing a role. Anyone can fetch projects and blog posts.
Write requests require `Editor` or `Admin`. Missing authentication returns
`401`. A signed-in guest gets `403`.

### 5. Run the tests

Start SQL Server and run from the repository root.

```powershell
dotnet test Portfolio.slnx
```

The xUnit project starts the real API and uses temporary SQL Server databases
with unique names. The configured SQL Server login needs permission to create
and drop those databases. Set `PORTFOLIO_TEST_CONNECTION` to use a separate SQL
Server connection for the tests. The tests register and log in over HTTP, and
the client keeps the authentication cookie when calling protected routes.

| Route | Access | Expected results | HTTP test |
| --- | --- | --- | --- |
| `GET /api/projects` | Public | `200` | `GetAllAndGetById_WithoutLogin_ReturnPublicItems` |
| `GET /api/projects/{id}` | Public | `200`, `404` | `GetAllAndGetById_WithoutLogin_ReturnPublicItems`, `UnknownId_ReturnsNotFoundProblemDetails` |
| `POST /api/projects` | Editor or Admin | `201`, `400`, `401`, `403` | `Post_ReturnsCreatedDtoAndLocation_AndPersistsItem`, `MissingRequiredFields_ReturnsBadRequest_AndDoesNotChangeExistingItem` |
| `PUT /api/projects/{id}` | Editor or Admin | `204`, `400`, `401`, `403`, `404` | `Put_ReturnsNoContent_AndPersistsChangedFields`, `MissingRequiredFields_ReturnsBadRequest_AndDoesNotChangeExistingItem`, `UnknownId_ReturnsNotFoundProblemDetails` |
| `DELETE /api/projects/{id}` | Editor or Admin | `204`, `401`, `403`, `404` | `Delete_RemovesItem_AndSubsequentGetReturnsNotFound`, `UnknownId_ReturnsNotFoundProblemDetails` |
| `GET /api/blogposts` | Public | `200` | `GetAllAndGetById_WithoutLogin_ReturnPublicItems` |
| `GET /api/blogposts/{id}` | Public | `200`, `404` | `GetAllAndGetById_WithoutLogin_ReturnPublicItems`, `UnknownId_ReturnsNotFoundProblemDetails` |
| `POST /api/blogposts` | Editor or Admin | `201`, `400`, `401`, `403` | `Post_ReturnsCreatedDtoAndLocation_AndPersistsItem`, `MissingRequiredFields_ReturnsBadRequest_AndDoesNotChangeExistingItem` |
| `PUT /api/blogposts/{id}` | Editor or Admin | `204`, `400`, `401`, `403`, `404` | `Put_ReturnsNoContent_AndPersistsChangedFields`, `MissingRequiredFields_ReturnsBadRequest_AndDoesNotChangeExistingItem`, `UnknownId_ReturnsNotFoundProblemDetails` |
| `DELETE /api/blogposts/{id}` | Editor or Admin | `204`, `401`, `403`, `404` | `Delete_RemovesItem_AndSubsequentGetReturnsNotFound`, `UnknownId_ReturnsNotFoundProblemDetails` |
| `POST /api/auth/register` | Public | `201`, `400`, `409` | `Register_WithRequestedAdminRole_CreatesGuestOnly`, `Register_WithInvalidInput_ReturnsBadRequest`, `Register_WithExistingEmail_ReturnsConflict` |
| `POST /api/auth/login` | Public | `204` with cookie, `400`, `401` | `Login_WithValidOrInvalidPassword_OnlyIssuesCookieForValidCredentials` |

`Write_WithoutLoginOrAsGuest_ReturnsUnauthorizedOrForbidden` verifies `401`
and `403` on all six write routes.
