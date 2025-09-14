using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SubscriptionTracker.Application.Services;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Add JWT bearer definition so Swagger UI shows the Authorize button
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: 'Bearer eyJhbGci...'",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});

// JWT Authentication configuration
var jwtKey = builder.Configuration["JWT:Key"];
var jwtIssuer = builder.Configuration["JWT:Issuer"];
var jwtAudience = builder.Configuration["JWT:Audience"];

if (string.IsNullOrEmpty(jwtKey))
{
    Console.WriteLine("Warning: JWT:Key is not configured. Authentication will fail unless set via configuration or environment variables.");
}

byte[] signingKeyBytes = Array.Empty<byte>();
if (!string.IsNullOrEmpty(jwtKey))
{
    // Only treat config value as Base64 when it contains typical base64 chars or padding
    if (jwtKey.IndexOfAny(new[] { '+', '/', '=' }) >= 0)
    {
        try
        {
            signingKeyBytes = Convert.FromBase64String(jwtKey);
        }
        catch
        {
            signingKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
        }
    }
    else
    {
        signingKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
    }

    // If the configured key is too short for HS256, derive a 256-bit key via SHA-256 to satisfy validation requirements
    if (signingKeyBytes.Length < 32)
    {
        signingKeyBytes = SHA256.HashData(signingKeyBytes);
    }

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
            ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
            ValidIssuer = jwtIssuer,
            ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        // Add policies here if needed. Example:
        // options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    });
}

// Register application services (concrete + interface bindings)
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IHouseholdService, HouseholdService>();
builder.Services.AddScoped<IHouseholdMemberService, HouseholdMemberService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<ISharedSubscriptionService, SharedSubscriptionService>();
builder.Services.AddScoped<ISubscriptionCategoryService, SubscriptionCategoryService>();
builder.Services.AddScoped<ISubscriptionPaymentService, SubscriptionPaymentService>();
builder.Services.AddScoped<ISubscriptionRatingService, SubscriptionRatingService>();
builder.Services.AddScoped<ISubscriptionUsageService, SubscriptionUsageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Connection string from configuration (e.g., appsettings.json or environment variable)
var connectionString = builder.Configuration.GetConnectionString("Default") ?? builder.Configuration["DefaultConnection"];
Console.WriteLine($"Using connection string: {connectionString}");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddSingleton<IDbConnectionFactory>(new NpgsqlConnectionFactory(connectionString));
    builder.Services.AddSingleton<IDbTransactionScopeFactory, DbTransactionScopeFactory>();

    // Unit of Work factory and UnitOfWork registration.
    // Repositories are intentionally NOT registered directly to avoid accidental resolution outside a UnitOfWork.
    builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
}
else
{
    // No connection string provided — register factories that fail fast when repositories are requested.
    // This forces the operator to configure a valid connection string in configuration or environment.
    // No DB: fail fast at the factory/unit-of-work level (not for every repository)
    builder.Services.AddSingleton<IDbConnectionFactory>(_ => throw new InvalidOperationException(
        "No database configured. Set 'DefaultConnection' in configuration to enable Dapper repositories."));
    builder.Services.AddScoped<IUnitOfWorkFactory>(_ => throw new InvalidOperationException(
        "No database configured. Set 'DefaultConnection' in configuration to enable Dapper repositories."));
    // Note: IUnitOfWork is intentionally not registered directly; use IUnitOfWorkFactory to create scoped UoW instances.
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable Swagger UI in all environments for easier testing (adjust for production as needed).
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Persist the entered bearer token so the user doesn't need to re-enter between requests
    c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
});

app.UseHttpsRedirection();
// Ensure authentication runs before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

