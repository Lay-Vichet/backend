using SubscriptionTracker.Application.Services;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure;
using SubscriptionTracker.Infrastructure.Dapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Connection string from configuration (e.g., appsettings.json or environment variable)
var connectionString = builder.Configuration.GetConnectionString("Default") ?? builder.Configuration["DefaultConnection"];
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddSingleton<IDbConnectionFactory>(new NpgsqlConnectionFactory(connectionString));

    // Transaction scope factory (allows service-level transaction composition)
    builder.Services.AddScoped<SubscriptionTracker.Application.Interfaces.IDbTransactionScopeFactory, SubscriptionTracker.Infrastructure.Dapper.DbTransactionScopeFactory>();

    // Unit of Work factory and UnitOfWork registration.
    // Repositories are intentionally NOT registered directly to avoid accidental resolution outside a UnitOfWork.
    builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
}
else
{
    // No connection string provided — register factories that fail fast when repositories are requested.
    // This forces the operator to configure a valid connection string in configuration or environment.
    // No DB: fail fast at the factory/unit-of-work level (not for every repository)
    builder.Services.AddSingleton<IDbConnectionFactory>(_ => throw new InvalidOperationException(
        "No database configured. Set 'DefaultConnection' in configuration to enable Dapper repositories."));
    builder.Services.AddScoped<SubscriptionTracker.Application.Interfaces.IDbTransactionScopeFactory>(_ => throw new InvalidOperationException(
        "No database configured. Set 'DefaultConnection' in configuration to enable Dapper repositories."));
    builder.Services.AddScoped<IUnitOfWorkFactory>(_ => throw new InvalidOperationException(
        "No database configured. Set 'DefaultConnection' in configuration to enable Dapper repositories."));
    builder.Services.AddScoped<IUnitOfWork>(_ => throw new InvalidOperationException(
        "No database configured. Set 'DefaultConnection' in configuration to enable Dapper repositories."));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

