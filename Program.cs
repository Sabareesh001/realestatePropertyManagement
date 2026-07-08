using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Repositories;
using propertyManagement.Services;
using propertyManagement.Filters;
using propertyManagement.Models;
using FluentValidation;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using propertyManagement.Hubs;

var builder = WebApplication.CreateBuilder(args);

var webRootPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
if (!Directory.Exists(webRootPath))
{
    Directory.CreateDirectory(webRootPath);
}
builder.Environment.WebRootPath = webRootPath;

var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:4200";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

// Configure DbContext with PostgreSQL
builder.Services.AddDbContext<PropertyManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register User Service
builder.Services.AddScoped<IUserService, UserService>();

// Register User Verification Service
builder.Services.AddScoped<IUserVerificationService, UserVerificationService>();

// Register Property Service
builder.Services.AddScoped<IPropertyService, PropertyService>();

// Register Lease Proposal Service
builder.Services.AddScoped<ILeaseProposalService, LeaseProposalService>();

// Register Lease Service
builder.Services.AddScoped<ILeaseService, LeaseService>();

// Register Bank Account Service
builder.Services.AddScoped<IBankAccountService, BankAccountService>();

// Register Charge & Payment Service
builder.Services.AddScoped<IChargePaymentService, ChargePaymentService>();

// Register Admin Finance Service
builder.Services.AddScoped<IAdminFinanceService, AdminFinanceService>();

// Register Complaint Service
builder.Services.AddScoped<IComplaintService, ComplaintService>();

// Register SignalR and the Notification Service
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register Stripe Connect Service
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddScoped<IStripeConnectService, StripeConnectService>();

// Register Stripe client and gateway via DI.
// IStripeClient holds the API key + HTTP client; IStripeGateway groups all Stripe SDK
// service classes (Accounts, PaymentIntents, Transfers, etc.) the same way
// IUnitOfWork groups database repositories.
var stripeSecretKey = builder.Configuration["Stripe:SecretKey"];
if (string.IsNullOrEmpty(stripeSecretKey))
{
    stripeSecretKey = "sk_test_dummy";
}
builder.Services.AddSingleton<Stripe.IStripeClient>(new Stripe.StripeClient(stripeSecretKey));
builder.Services.AddSingleton<IStripeGateway, StripeGateway>();

// Register JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured.");

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            else if (context.Request.Cookies.TryGetValue("jwt_token", out var token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
