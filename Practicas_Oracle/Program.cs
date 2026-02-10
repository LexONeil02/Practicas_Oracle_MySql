using Microsoft.EntityFrameworkCore;
using Practicas_Oracle.MiddleWare;
using Practicas_Oracle.Models;
using Serilog;

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Sigue mostrando logs en la consola
    .WriteTo.File("Logs/migracion-.txt",
        rollingInterval: RollingInterval.Day) // Crea un archivo nuevo cada día
    .CreateLogger();



var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog(); // Le dice a ASP.NET Core que use Serilog


// Add services to the container.
var mySqlConn = builder.Configuration.GetConnectionString("MySqlConnection");
var oracleConn = builder.Configuration.GetConnectionString("OracleConnection");

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMigracionServices, MigracionServices>();

// Configuración para MySQL 
builder.Services.AddDbContext<AcademiaContext>(options =>
    options.UseMySql(mySqlConn, ServerVersion.AutoDetect(mySqlConn)));

// Configuración para Oracle 
builder.Services.AddDbContext<OracleAcademiaContext>(options =>
    options.UseOracle(oracleConn));
builder.Services.AddControllers();



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
