using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementApp.API.Middleware;
using QuantityMeasurementApp.Business;
using QuantityMeasurementApp.Business.Implementations;
using QuantityMeasurementApp.Interface.Repository;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 🔥 FORCE connection string (prevents tcp:// override issue)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Debug print (VERY IMPORTANT - remove later)
Console.WriteLine("🔍 CONNECTION STRING => " + connectionString);

// 🔍 DIAGNOSTIC: Check where it's coming from
foreach (var source in builder.Configuration.AsEnumerable())
{
    if (source.Key.Contains("DefaultConnection"))
    {
        Console.WriteLine($"   📍 Source Key: {source.Key} => {source.Value}");
    }
}

// ❌ Safety check (kills app if wrong format)
if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("tcp://"))
{
    throw new Exception("❌ Invalid connection string detected. Fix your configuration.");
}

// ── Database Configuration ─────────────────────────────
builder.Services.AddDbContext<QuantityMeasurementDbContext>(
    options => options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null
            );
        })
);

// ── Repositories ──────────────────────────────────────
builder.Services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementEFRepository>();
builder.Services.AddScoped<IUserRepository, UserEFRepository>();

// ── Services ──────────────────────────────────────────
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>();
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();

// ── AES Encryption Service ────────────────────────────
builder.Services.AddScoped<AesEncryptionService>();

// ── CORS Configuration ────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500",
                "http://localhost:3000",
                "http://127.0.0.1:3000",
                "http://localhost:4200",
                "http://127.0.0.1:4200",
                "null"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ── JWT Authentication ────────────────────────────────
string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// ── Controllers + Swagger ─────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Quantity Measurement API",
        Version = "v1",
        Description = "JWT Auth + AES-256 Encryption + Quantity Measurement REST API"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Paste your JWT token here"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────
app.UseCors("FrontendPolicy");

app.UseMiddleware<GlobalExceptionHandler>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }