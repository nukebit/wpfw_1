# Brian Arjun portfolio

A React portfolio built with Vite and React Router, connected to the existing
ASP.NET Core Identity API and SQL Server database. The code and comments are in
English. The React structure follows the MovieLab examples from lessons 9 and 10.

## Requirements and local startup

Install Node.js 22.12 or newer (Node 22.20 is suitable), the .NET 10 SDK and
SQL Server Database Engine. Keep SQL Server (MSSQLSERVER) running.

### 1. Database and API

The default connection in `backend/appsettings.json` uses Windows authentication:

```text
Server=localhost;Database=WpfwPortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;
```

Use the ignored `backend/appsettings.Local.json` or the
`ConnectionStrings__PortfolioDatabase` environment variable for machine-specific
settings. Never commit passwords or production connection strings.

From the repository root:

```powershell
dotnet restore Portfolio.slnx
dotnet run --project backend
```

The Development profile applies EF Core migrations and seeds empty tables.
The API runs at http://localhost:5074.
Open [Swagger](http://localhost:5074/swagger/index.html).

For a database that predates EF migrations, follow the checks in
`backend/Data/BaselineExistingDatabase.sql` before applying migrations.
Do not drop an existing database to work around a connection error.

SQL Server can also run through `backend/compose.yaml`. Supply your own strong
`MSSQL_SA_PASSWORD` in the ignored `backend/.env`, run
`docker compose -f backend/compose.yaml up -d`, and configure the corresponding
SQL login connection locally. The Docker database is separate from an existing
Windows SQL Server database. Use a free host port if port 1433 is already in use.

### 2. React

In a second terminal:

```powershell
cd frontend
npm install
npm run dev
```

Open [the portfolio](http://127.0.0.1:5173). Vite forwards `/api` to
`http://localhost:5074`. React uses relative API URLs with
`credentials: "include"`; the browser stores the Identity cookie for the
frontend origin. Use the same hostname throughout the demo.

All portfolio lists and details come from the API and SQL Server. There is no
local JSON fallback. The home page preserves the original Open-Meteo weather
feature and the contact demonstration form.

### 3. Accounts and test roles

Register three accounts through React: one guest, one editor and one admin.
Registration always assigns `Gast`; users cannot request an elevated role.
Sign in with the guest to verify the read-only interface.

Stop the API before using the existing local role assignment command:

```powershell
dotnet run --project backend -- --assign-role editor@example.test Editor
dotnet run --project backend -- --assign-role admin@example.test Admin
```

Replace the example emails with accounts you registered. Start the API again,
then sign out and sign in again to refresh the roles in the cookie.

| User | Lists and details | POST, PUT and DELETE |
| --- | --- | --- |
| Visitor | Public | 401 |
| Gast | Public | 403 |
| Editor | Public | Allowed |
| Admin | Public | Allowed |

`GET /api/auth/me` returns `id`, `email` and `roles`. It returns 401 without a
valid session. React calls it after login and on every page reload.
`POST /api/auth/logout` removes the session cookie. Write access remains
protected by the API's `CanEditPortfolio` policy.

## Routes and code structure

| Browser route | Purpose |
| --- | --- |
| `/` | About, live weather and contact demonstration |
| `/projects` | Project list and Editor/Admin management |
| `/projects/:id` | Project detail |
| `/blogposts` | Blog list and Editor/Admin management |
| `/blogposts/:id` | Blog detail |
| `/register` | Register a Gast |
| `/login` | Cookie-based sign in |
| Any other route | Page not found |

- `src/api/client.js`: fetch, cookies, empty responses and readable HTTP errors.
- `src/api/projects.js`, `blogposts.js`, `auth.js`: endpoint-specific functions.
- `src/auth/AuthContext.jsx`: session state, reload lookup and `canEdit`.
- `src/hooks/useApiResource.js`: loading, empty, error and missing item states.
  Effect cleanup aborts old requests; the resource key also prevents a stale
  detail from appearing during an ID change.
- `src/components/ResourceManager.jsx`: the lesson's list/form/save/delete flow,
  shared by the project and blog overview.
- `ProjectForm` and `BlogPostForm`: controlled fields, DTO length limits,
  whitespace validation, server errors, labels and busy states.
- `ProjectCard` and `BlogPostCard`: props and links to details.

GET flow: page → API function → fetch client → controller → service →
repository → EF Core → SQL Server → JSON → React state → cards.

Write flow: form → save callback → POST/PUT/DELETE → Identity cookie
authentication → role policy → DTO validation → service/repository/EF Core →
SQL Server → response → overview reload. A native confirmation appears before
DELETE. Labels, focus outlines, semantic HTML, status messages and an alert for
errors support keyboard and screen reader use.

## Verification

Run from the repository root:

```powershell
dotnet test Portfolio.slnx
```

The xUnit integration tests use temporary SQL Server databases with unique names.
The SQL login needs permission to create and drop those test databases.
Set `PORTFOLIO_TEST_CONNECTION` to use a separate SQL Server test connection.
Never point a manual delete command at your portfolio database.

Run from `frontend`:

```powershell
npm test
npm run build
```

The dependency-free client tests can also run from the root with
`node --test frontend/tests/*.test.js`. They check response handling, 401/403
messages, validation, cancellation and both resources' write methods. These
mock-fetch checks do not prove a database write succeeded; use the integration
tests and browser scenarios for that.

### Browser scenarios for Brightspace

1. Open both public lists and a detail from each. Enter an unknown ID and an
   unknown browser route. Use DevTools network throttling to inspect loading.
   Stop the API briefly to inspect the retry message; restart it afterward.
   An empty list can be checked in a separate empty test database.
2. Register a Gast, sign in, reload and confirm the same account and role.
   Sign out and confirm the session disappears.
3. In DevTools Console at the React origin, run this valid protected request:

```javascript
const response = await fetch("/api/projects", {
  method: "POST",
  credentials: "include",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    title: "Permission check",
    type: "Test",
    description: "A valid DTO for the authorization check."
  })
});
console.log(response.status);
```

Without login this must return 401. Repeat as Gast for 403.
Avoid running this diagnostic as Editor/Admin unless you want to create an item.

4. As Editor, create a project and a blog post, reload and open their details.
   Edit both, reload and check the saved values. Cancel a delete, then confirm
   a delete and reload. Repeat a successful write as Admin.
5. With DevTools Slow 3G enabled, change between detail IDs quickly. Only the
   currently selected item should remain visible.
6. Capture the request method, URL, payload and response for one GET and one
   write. Redact passwords and cookie values from screenshots.

## Cookies and CSRF before deployment

The local Development cookie uses `HttpOnly`, `SameSite=Lax` and
`SecurePolicy=SameAsRequest` so the local HTTP proxy can be used. Other
environments require a Secure cookie.

Before deploying, serve frontend and API over HTTPS, configure trusted proxy
headers and shared data-protection keys, and decide on the final cookie domain
and SameSite policy. Keep the app on one origin when practical. If different
origins are required, allow only the actual frontend origin with credentials;
never combine wildcard origins with credentialed CORS.

Add ASP.NET Core antiforgery protection for cookie-authenticated mutations,
including authentication actions, and send the request token in a header from
the API client. Keep the authentication cookie HttpOnly and validate tokens on
the server. SameSite is defense in depth, not a replacement for CSRF protection.
Do not use `SameSite=None` without HTTPS and deliberate CSRF protection.

Passwords exist only as transient form values and request bodies. Neither
passwords nor session tokens are stored in localStorage or sessionStorage.
Do not log credentials or expose database secrets through Vite environment
variables.

## Sources

- The assignment 5 rubric and WPFW lessons 9 and 10 supplied with the task.
- [React useEffect](https://react.dev/reference/react/useEffect)
- [React Router declarative installation](https://reactrouter.com/start/declarative/installation)
- [Vite guide](https://vite.dev/guide/)
- [Vite proxy configuration](https://vite.dev/config/server-options#server-proxy)
- [ASP.NET Core antiforgery](https://learn.microsoft.com/aspnet/core/security/anti-request-forgery)
