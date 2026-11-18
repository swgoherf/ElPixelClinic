using MassTransit;
using Microsoft.EntityFrameworkCore;
using Office.Api.Exceptions;
using Office.Application.Common.Interfaces;
using Office.Application.Services;
using Office.Infrastructure.Data;
using Office.Infrastructure.Events;
using Office.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OfficeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

builder.Services.AddScoped<IOfficeRepository, OfficeRepository>();
builder.Services.AddScoped<IOfficeEventPublisher, MassTransitOfficeEventPublisher>();
builder.Services.AddScoped<IOfficeService, OfficeService>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitConfig = builder.Configuration.GetSection("RabbitMq");
        var host = rabbitConfig.GetValue<string>("Host") ?? "localhost";
        var port = rabbitConfig.GetValue<ushort>("Port", 5672);
        var username = rabbitConfig.GetValue<string>("Username") ?? "guest";
        var password = rabbitConfig.GetValue<string>("Password") ?? "guest";

        cfg.Host(host, port, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

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
