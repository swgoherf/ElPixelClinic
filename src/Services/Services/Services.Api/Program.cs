using Microsoft.EntityFrameworkCore;
using Services.Api.Exceptions;
using Services.Application.Common.Interfaces;
using Services.Application.Services;
using Services.Infrastructure.Data;
using Services.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ServicesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

builder.Services.AddScoped<IMedicalServiceRepository, MedicalServiceRepository>();
builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();

builder.Services.AddScoped<IMedicalServiceService, MedicalServiceService>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
