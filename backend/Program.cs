using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Repositories;
using PortfolioApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connect EF Core to the SQL Server database configured in appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("PortfolioDatabase")
    ?? throw new InvalidOperationException("Connection string 'PortfolioDatabase' is missing.");

builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(connectionString));

// Create the repositories and services through dependency injection for each request.
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<BlogPostRepository>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogPostService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Create the database and add example data if the tables are empty.
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    DbInitializer.Initialize(context);

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Match incoming requests to the routes defined in the controllers.
app.UseRouting();
app.MapControllers();
app.Run();
