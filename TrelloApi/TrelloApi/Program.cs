using System.Text;
using System.Threading.RateLimiting;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TrelloApi.app;
using TrelloApi.Application.Filters;
using TrelloApi.Application.Mappings;
using TrelloApi.Application.Middlewares;
using TrelloApi.Infrastructure.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Autenticación y Autorización
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// Configurar Controladores y Filtros Globales
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<LoggingFilter>();
});

// Configurar FluentValidation
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

// Registro de Validadores
builder.Services.AddValidatorsFromAssemblies(); 

// Configuración de Logging
builder.Services.AddLogging();

// Configuración de Base de Datos
builder.Services.AddDbContext<TrelloContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("TrelloConnection"));
});

// Registro de Servicios de Aplicación
builder.Services.AddApplicationServices();
builder.Services.AddApplicationRepositories();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityDefinition = new OpenApiSecurityScheme
    {
        Name = "Bearer",
        BearerFormat = "JWT",
        Scheme = "bearer",
        Description = "Specify the authorization token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    };
    c.AddSecurityDefinition("Jwt Auth", securityDefinition);

    var securityScheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Id = "Jwt Auth",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

// Configuración de AutoMapper
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Rate limit
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("fixed", httContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(10)
            }));
    options.AddPolicy("block", httContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(5)
            }));
});

var app = builder.Build();

app.UseCors("AllowAngularApp");

// Aplicar Migraciones Solo en Desarrollo
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        TrelloContext context = scope.ServiceProvider.GetRequiredService<TrelloContext>();
        
        if (context.Database.IsRelational() && context.Database.CanConnect())
        {
            context.Database.Migrate();
        }
    }
}

// Middleware y Configuración de API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ResponseEmitterMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();

public partial class Program {}
