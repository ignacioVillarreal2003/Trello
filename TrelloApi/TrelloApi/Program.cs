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
using TrelloApi.Application.Hub;
using TrelloApi.Application.Mappings;
using TrelloApi.Application.Middlewares;
using TrelloApi.Application.Utils;
using TrelloApi.Infrastructure.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                     ?? new string[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BoardAccessPolicy", policy =>
        policy.Requirements.Add(new BoardAccessRequirement()));
});
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

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<LoggingFilter>();
});

builder.Services.AddSignalR();

builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblies(); 

builder.Services.AddLogging();

builder.Services.AddDbContext<TrelloContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("TrelloConnection"));
});

builder.Services.AddApplicationServices();
builder.Services.AddApplicationRepositories();

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

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("fixed", httContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromSeconds(10)
            }));
    options.AddPolicy("block", httContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1)
            }));
});

var app = builder.Build();

app.UseCors("AllowAngularApp");

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ResponseEmitterMiddleware>();
app.UseMiddleware<BoardCookieMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.MapHub<BoardHub>("/boardHub");

app.Run();

public partial class Program {}
