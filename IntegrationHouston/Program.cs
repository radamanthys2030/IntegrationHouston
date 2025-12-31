using Integration.Houston.Application;
using Integration.Houston.Application.Contract;
using Integration.Houston.Application.Infrastructure.EntityFramework;
using Integration.Houston.Application.Infrastructure.Mapper;
using Integration.Houston.Application.Infrastructure.Repositorie;
using Integration.Houston.Application.Infrastructure.Services;
using IntegrationHouston;
using IntegrationHouston.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddTransient<ITransactionRepositorie, TransactionRepositorie>();
builder.Services.AddTransient<IserviceTransaction, ServiceTransaction>();
builder.Services.AddTransient<IApplicationContract, TransactionApplication>();




builder.Services.AddDbContext<TransactiondbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Default");

    // SQL Server:
    // options.UseSqlServer(cs);

    // PostgreSQL:
    options.UseNpgsql(cs);

    // SQLite:
    // options.UseSqlite(cs);

    // Opcional: logging de SQL y sensibilidad de datos
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});




// 1) Configurar Authentication con esquema JWT
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // true en producción si usas HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2) // tolerancia
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdmins", policy => policy.RequireRole("Admin"));
});

// Swagger con soporte para Bearer
builder.Services.AddEndpointsApiExplorer();   
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT en formato: Bearer {token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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


app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<BasicAuthMiddleware>();
app.MapPost("api/v1/Security/login", (ExternalLogin req) =>
{

    
    // Aquí validas usuario/contraseña contra tu base de datos
    if (req.User == "demo" && req.Password == "P@ssw0rd!")
    {
        var token = JwtTokenHelper.GenerateToken(
            issuer: jwt["Issuer"]!,
            audience: jwt["Audience"]!,
            key: jwt["Key"]!,
            subjectId: "user-123",
            username: req.User,
            roles: new[] { "User", "Admin" },      // roles opcionales
            customClaims: new Dictionary<string, string> { { "ClienteId", "c-001" } },
            expiresMinutes: int.Parse(jwt["AccessTokenMinutes"]!)
        );

        return Results.Ok(new { access_token = token, token_type = "Bearer" });
    }

    return Results.Unauthorized();
});




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI();

 
app.MapControllers();

app.Run();
