using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Repositories;
using PortfolioApi.Security;
using PortfolioApi.Services;

var assignRole = args.FirstOrDefault() == "--assign-role";
var builder = WebApplication.CreateBuilder(assignRole ? [] : args);

// Load this computer's database settings without putting its password in Git.
if (builder.Environment.IsDevelopment())
{
    builder.Configuration
        .AddJsonFile("appsettings.Local.json", optional: true)
        .AddEnvironmentVariables();
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connect EF Core to the SQL Server database configured in appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("PortfolioDatabase")
    ?? throw new InvalidOperationException("Connection string 'PortfolioDatabase' is missing.");

builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    options.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<PortfolioDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Portfolio.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    // API clients need status codes rather than redirects to HTML account pages.
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(PortfolioRoles.CanEditPortfolio, policy =>
        policy.RequireRole(PortfolioRoles.Editor, PortfolioRoles.Admin));

// Create the repositories and services through dependency injection for each request.
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();

var app = builder.Build();

if (assignRole)
{
    Environment.ExitCode = await LocalRoleAssignment.RunAsync(app, args);
    await app.DisposeAsync();
    return;
}

if (app.Environment.IsDevelopment())
{
    // Apply migrations and add example data if the tables are empty.
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    await DbInitializer.InitializeAsync(context);

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Match incoming requests to the routes defined in the controllers.
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Expose the entry point to the HTTP integration tests.
public partial class Program { }
