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

### 3. Run the backend

Open a terminal in the main project folder. Run these commands in order:

```powershell
cd backend
dotnet restore
dotnet run
```

The backend creates the database and tables automatically when needed. Empty tables get example projects and blogposts. Saved data stays in SQL Server after a restart. Changes to existing table structures must be applied separately.

### 4. Test the API

Open [Swagger](http://localhost:5074/swagger/index.html) in your browser:

1. Choose a route and click **Try it out**.
2. Fill in any required values, such as a project ID. For POST or PUT, fill in the example JSON.
3. Click **Execute** to see the result.

To stop the backend, press **Ctrl+C** in the terminal.

The detailed explanation, diagram and API screenshots are in the WPFW document.
