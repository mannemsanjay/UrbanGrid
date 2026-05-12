using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using DotNetEnv;
using UrbanGrid.API.Middleware;
using UrbanGrid.Application.Interfaces;
using UrbanGrid.Application.Services;
using UrbanGrid.Core.Interfaces;
using UrbanGrid.Infrastructure.Data;
using UrbanGrid.Infrastructure.Repositories;
using UrbanGrid.Infrastructure.Services;

// Load .env file
if (File.Exists(".env"))
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/urbangrid-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 🔍 DEBUG — shows which connection string is being used
Console.WriteLine("🔍 DB Connection: " +
    builder.Configuration.GetConnectionString("DefaultConnection"));

// Database
builder.Services.AddDbContext<UrbanGridDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IFaultRepository, FaultRepository>();
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IFaultService, FaultService>();
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// JWT Authentication
var jwtKey = builder.Configuration["JwtSettings:SecretKey"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Role-based authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireRole("ADMIN"));
    options.AddPolicy("StaffOnly",
        policy => policy.RequireRole(
            "ADMIN", "DISPATCHER", "TECHNICIAN",
            "ASSET_MANAGER", "PROCUREMENT", "FINANCE", "COMPLIANCE"));
    options.AddPolicy("CitizenOrAbove",
        policy => policy.RequireRole(
            "CITIZEN", "ADMIN", "DISPATCHER", "TECHNICIAN",
            "ASSET_MANAGER", "PROCUREMENT", "FINANCE", "COMPLIANCE"));
});

// CORS — allow Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:80")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        // ✅ ADD THIS
        opts.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UrbanGrid API", Version = "v1",
        Description = "Municipal Streetlight Asset Management System"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                    { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrbanGrid API v1"));
}

app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseCors("AngularPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ✅ Auto-migrate + seed on startup (non-fatal)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UrbanGridDbContext>();
    Console.WriteLine("Migrating database...");
    try
    {
        db.Database.Migrate();
        Console.WriteLine("✅ Database migration complete!");
        await DbSeeder.SeedAsync(db);
        Console.WriteLine("✅ Seeding complete!");
    }
    catch (Exception ex)
    {
        // ✅ Don't crash — migrations already applied manually via Neon SQL
        Console.WriteLine($"⚠️ Migration skipped: {ex.Message}");

        // ✅ Still run seeder separately
        try
        {
            await DbSeeder.SeedAsync(db);
            Console.WriteLine("✅ Seeding complete!");
        }
        catch (Exception seedEx)
        {
            Console.WriteLine($"⚠️ Seeding skipped: {seedEx.Message}");
        }
    }
}

app.Run();
