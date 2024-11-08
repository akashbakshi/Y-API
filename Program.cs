using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.IdentityModel.Tokens;
using YApi.Data;
using YApi.Models;
using YApi.Services;

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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwtOptions =>
    {
        
        
        jwtOptions.SaveToken = true;
        jwtOptions.RequireHttpsMetadata = false;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

builder.Services.AddSingleton<JwtService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("YApiCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("access-token");
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("YApiCorsPolicy");

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

string version = "V0.6";
Console.WriteLine($"YApi Service version {version} running...");

app.Run();
