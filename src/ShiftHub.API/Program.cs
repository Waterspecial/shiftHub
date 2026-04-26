using Microsoft.EntityFrameworkCore;
using ShiftHub.Application.Interfaces;
using ShiftHub.Infrastructure.Persistence;
using ShiftHub.Infrastructure.Services;
using ShiftHub.Infrastructure.Services.Organisations;

var builder = WebApplication.CreateBuilder(args);

// HTTP context accessor — needed by CurrentTenantService to read the JWT token
builder.Services.AddHttpContextAccessor();

// Tenant service — reads OrgId and UserId from the JWT on every request
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();

// Auth service — handles register, login, and workspace selection
builder.Services.AddScoped<IAuthService, AuthService>();

// Organisation service — handles agency creation and member management
builder.Services.AddScoped<IOrganisationService, OrganisationService>();

// Database — connects EF Core to PostgreSQL
builder.Services.AddDbContext<ShiftHubDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// Swagger — lets you test the API in the browser
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
