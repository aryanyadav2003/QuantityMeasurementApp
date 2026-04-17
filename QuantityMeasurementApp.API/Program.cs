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

// ── Database — Always SQL Server (data persists to SSMS) ─
builder.Services.AddDbContext<QuantityMeasurementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Repositories ──────────────────────────────────────
builder.Services.AddScoped<IQuantityMeasurementRepository,
    QuantityMeasurementEFRepository>();
builder.Services.AddScoped<IUserRepository,
    UserEFRepository>();

// ── Services ──────────────────────────────────────────
builder.Services.AddScoped<IQuantityMeasurementService,
    QuantityMeasurementServiceImpl>();
builder.Services.AddScoped<IAuthService,
    AuthServiceImpl>();

// ── AES Encryption Service ────────────────────────────
builder.Services.AddScoped<AesEncryptionService>();

// ── CORS — allow frontend ─────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5500",    // VS Code Live Server
                "http://127.0.0.1:5500",
                "http://localhost:3000",
                "http://127.0.0.1:3000",
                "http://localhost:4200",
                "http://127.0.0.1:4200",
                "null")                     // file:// protocol
            .AllowAnyHeader()
            .AllowAnyMethod();
            // NOTE: AllowCredentials() removed — incompatible with "null" origin
    });
});

// ── JWT Authentication ────────────────────────────────
string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],
        ValidAudience            = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(
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
        Title       = "Quantity Measurement API",
        Version     = "v1",
        Description = "UC18 — JWT Auth + AES-256 Encryption + Quantity Measurement REST API"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Paste your JWT token here. Example: eyJhbGci..."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── CORS must be first to ensure headers are added to all responses (including errors)
app.UseCors("FrontendPolicy");

app.UseMiddleware<GlobalExceptionHandler>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }