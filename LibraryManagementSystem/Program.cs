using LibraryManagementSystem.Persistence;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Web.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbContext"));
});
builder.Services
    .AddScoped<MembersService>()
    .AddScoped<CategoryService>()
    .AddScoped<BooksService>()
    .AddScoped<BookIssueService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

var apigroup = app.MapGroup("api");
apigroup.MapMembersEndpoints();
apigroup.MapCategoryEndpoints();
apigroup.MapBooksEndpoints();
apigroup.MapBookIssueEndpoints();

app.MapGet("/", () => $"Running in {app.Environment.EnvironmentName} right now.");

app.Run();