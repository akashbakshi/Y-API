using Microsoft.AspNetCore.Identity;
using YApi.Data;
using YApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
string connectionString = builder.Configuration.GetConnectionString(builder.Environment.EnvironmentName) ?? "";

if (String.IsNullOrEmpty(connectionString))
{
    Console.WriteLine($"No connection string found for '{builder.Environment.EnvironmentName}'");
}

builder.Services.AddNpgsql<YDbContext>(connectionString);

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<YDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();
