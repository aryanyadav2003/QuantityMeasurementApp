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

bool isEfToolsRunning = Environment.GetEnvironmentVariable("EF_TOOLS") == "true";

// ── Database ──────────────────────────────────────────
if (isEfToolsRunning)
{
    builder.Services.AddDbContext<QuantityMeasurementDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    builder.Services.AddDbContext<QuantityMeasurementDbContext>(options =>
        options.UseInMemoryDatabase("QuantityMeasurementDB"));
}

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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title       = "Quantity Measurement API",
        Version     = "v1",
        Description = "UC18 — JWT Auth + AES-256 Encryption + Quantity Measurement REST API"
    });

    // JWT Authorize button in Swagger UI
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

app.UseMiddleware<GlobalExceptionHandler>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }