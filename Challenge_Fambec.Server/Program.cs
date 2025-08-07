using Microsoft.EntityFrameworkCore;
using Challenge_Fambec.Server.Data;
using Challenge_Fambec.Server.Services;
using DotNetEnv;

// Load .env file from project root (one level up from Server directory)
var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
var envPath = Path.Combine(projectRoot ?? "", ".env");

if (File.Exists(envPath))
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dynamic connection string based on environment
string connectionString = GetConnectionString(builder.Configuration);

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddHttpClient<OpenRouterService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", policy =>
    {
        policy.WithOrigins("https://localhost:5214", "http://localhost:5000", "https://localhost:7179")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowBlazorWasm");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Auto migration with error handling
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Using connection: {Server}", connectionString.Split(';')[0]);
        await context.Database.CanConnectAsync();
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migration successful");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed");
        if (!app.Environment.IsDevelopment()) throw;
    }
}

app.Run();

// Simple container detection
static string GetConnectionString(IConfiguration configuration)
{
    // Check if running in Docker container
    bool isContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != null ||
                      File.Exists("/.dockerenv");

    var baseConnection = configuration.GetConnectionString("DefaultConnection")!;
    
    // Use localhost if running on host, otherwise use docker service name
    return isContainer ? baseConnection : baseConnection.Replace("sqlserver", "localhost");
}