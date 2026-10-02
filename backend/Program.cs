using PortfolioApi.Repositories;
using PortfolioApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create the repositories and services through dependency injection for each request.
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<BlogPostRepository>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogPostService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Match incoming requests to the routes defined in the controllers.
app.UseRouting();
app.MapControllers();
app.Run();
