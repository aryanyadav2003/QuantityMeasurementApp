using Microsoft.EntityFrameworkCore;
using QuantityMeasurementApp.API.Middleware;
using QuantityMeasurementApp.Business;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// EF Core tools set "ASPNETCORE_ENVIRONMENT" to "Development"
// but also pass a special flag we can detect
bool isEfToolsRunning = EF.IsDesignTime;

if (isEfToolsRunning)
{
    // SqlServer required for migration generation
    builder.Services.AddDbContext<QuantityMeasurementDbContext>(options =>
        options.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;" +
            "Database=QuantityMeasurementDB;"  +
            "Trusted_Connection=True;"          +
            "TrustServerCertificate=True;"));
}
else
{
    // InMemory for normal app run
    builder.Services.AddDbContext<QuantityMeasurementDbContext>(options =>
        options.UseInMemoryDatabase("QuantityMeasurementDB"));
}

// EF Core Repository
builder.Services.AddScoped<IQuantityMeasurementRepository,
    QuantityMeasurementEFRepository>();

// Service
builder.Services.AddScoped<IQuantityMeasurementService,
    QuantityMeasurementServiceImpl>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();