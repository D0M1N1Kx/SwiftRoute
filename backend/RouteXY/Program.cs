using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using FluentValidation;
using RouteXY.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RouteXY.Api.Data;
using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Services;
using RouteXY.Api.Endpoints;
using RouteXY.Api.Entities;
using RouteXY.Api.Hubs;
var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RouteXY API",
        Version = "v1",
        Description = "Courier management system API"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token}"
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", null, null),
            new List<string>()
        }
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());

builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddSignalR();
builder.Services.AddScoped<LocationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapPost("/setup", async (AppDbContext db) =>
    {
        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Admin",
            Email = "admin@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("jelszo123"),
            Role = RouteXY.Api.Enums.UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Users.Add(admin);
        await db.SaveChangesAsync();
        return Results.Ok("Admin user created");
    });
}

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapWarehouseEndpoints();
app.MapInventoryItemEndpoints();
app.MapOrderEndpoints();
app.MapHub<LocationHub>("/hubs/location");
app.MapLocationEndpoints();

app.Run();