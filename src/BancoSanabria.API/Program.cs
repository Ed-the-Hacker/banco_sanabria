using BancoSanabria.API.Middleware;
using BancoSanabria.Application.Common.Interfaces;
using BancoSanabria.Application.Services;
using BancoSanabria.Application.Strategies;
using BancoSanabria.Infrastructure.Data;
using BancoSanabria.Infrastructure.Repositories;
using BancoSanabria.Infrastructure.Services;
using BancoSanabria.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de DbContext
builder.Services.AddDbContext<BancoDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
    // Descomentar para usar PostgreSQL:
    // options.UseNpgsql(connectionString);
});

// Registro de Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICuentaRepository, CuentaRepository>();
builder.Services.AddScoped<IMovimientoRepository, MovimientoRepository>();

// Registro de Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Registro de Servicios de Aplicación
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICuentaService, CuentaService>();
builder.Services.AddScoped<IMovimientoService, MovimientoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();

// Registro de Estrategias de Movimiento (Patrón Strategy)
builder.Services.AddScoped<IMovimientoStrategy, CreditoStrategy>();
builder.Services.AddScoped<IMovimientoStrategy, DebitoStrategy>();
builder.Services.AddScoped<MovimientoStrategyFactory>();

// Configuración de Controllers
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Banco Sanabria API",
        Version = "v1",
        Description = "API RESTful para gestión bancaria - Sistema de cuentas y movimientos"
    });
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware de manejo de excepciones global
app.UseExceptionHandlingMiddleware();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Banco Sanabria API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();

