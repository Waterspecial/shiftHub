using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShiftHub.Application.Interfaces;
using ShiftHub.Infrastructure.Persistence;
using ShiftHub.Infrastructure.Services;
using ShiftHub.Infrastructure.Services.Clients;
using ShiftHub.Infrastructure.Services.Organisations;
using ShiftHub.Infrastructure.Services.PayRates;
using ShiftHub.Infrastructure.Services.Shifts;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// HTTP context accessor — needed by CurrentTenantService to read the JWT token
builder.Services.AddHttpContextAccessor();

// Tenant service — reads OrgId and UserId from the JWT on every request
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();

// Auth service — handles register, login, and workspace selection
builder.Services.AddScoped<IAuthService, AuthService>();

// Organisation service — handles agency creation and member management
builder.Services.AddScoped<IOrganisationService, OrganisationService>();

// Client service — handles client management
builder.Services.AddScoped<IClientService, ClientService>();

// PayRate service — handles pay rate management
builder.Services.AddScoped<IPayRateService, PayRateService>();

// Shift service — handles shift lifecycle
builder.Services.AddScoped<IShiftService, ShiftService>();

// Database — connects EF Core to PostgreSQL
builder.Services.AddDbContext<ShiftHubDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
