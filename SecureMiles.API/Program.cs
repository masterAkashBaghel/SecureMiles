using System.Text;
using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;
using SecureMiles.Repositories;
using SecureMiles.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SecureMiles.Repositories.Vehicle;
using SecureMiles.Services.Vehicle;
using SecureMiles.Repositories.Policy;
using SecureMiles.Services.Policy;
using SecureMiles.Repositories.Proposals;
using SecureMiles.Services.Proposals;
using SecureMiles.Repositories.Claims;
using SecureMiles.Services.Claims;
using SecureMiles.API.Cloudinary;
using SecureMiles.Services.Cloudinary;
using SecureMiles.Repositories.Documents;
using SecureMiles.Services.Document;
using SecureMiles.Services.Mail;
using SecureMiles.Repositories.Admin;
using SecureMiles.Services.Admin;
// using SecureMiles.Repositories.Payment;
// using SecureMiles.Services.Payment;
using SecureMiles.Services.FakePayment;
using SecureMiles.Repositories.FakePayment;


var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddScoped<PayPalService>();
// builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddScoped<IPayPalService, PayPalService>();

builder.Services.AddSwaggerGen(c =>
{
    // Add a security definition for Bearer token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer' followed by your JWT token"
    });

    // Add security requirement to apply to all endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// logger configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read from appsettings.json
    .CreateLogger();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Set the minimum log level (e.g., Debug, Information, Warning)
    .WriteTo.Console()    // Write logs to the console
    .WriteTo.File("Logs/SecureMiles.log", rollingInterval: RollingInterval.Day) // Write logs to a file
    .CreateLogger();

// Replace the default logging with Serilog
builder.Host.UseSerilog();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKeyValue = jwtSettings["SecretKey"];
if (string.IsNullOrEmpty(secretKeyValue))
{
    throw new InvalidOperationException("JWT SecretKey is not configured.");
}
var secretKey = Encoding.UTF8.GetBytes(secretKeyValue);

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

builder.Services.AddAuthorization();

//cloudinary configuration
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton<CloudinaryService>();

// email configuration
builder.Services.AddSingleton<EmailService>();

// Register DbContext with migrations assembly
builder.Services.AddDbContext<InsuranceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("SecureMiles.API")));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<IProposalsRepository, ProposalsRepository>();
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IPolicyServices, PolicyService>();
builder.Services.AddScoped<IProposalService, ProposalService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureMiles API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

// Use CORS middleware
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();