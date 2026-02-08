using Microsoft.EntityFrameworkCore;
using Practicas_Oracle.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mySqlConn = builder.Configuration.GetConnectionString("MySqlConnection");
var oracleConn = builder.Configuration.GetConnectionString("OracleConnection");

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

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
