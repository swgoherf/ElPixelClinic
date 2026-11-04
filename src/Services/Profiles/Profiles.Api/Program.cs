using Dapper;
using MassTransit;
using Profiles.Api.Exceptions;
using Profiles.Application.Common.Interfaces;
using Profiles.Application.Services;
using Profiles.Infrastructure.Consumers;
using Profiles.Infrastructure.Data;
using Profiles.Infrastructure.Events;
using Profiles.Infrastructure.Repositories;

DefaultTypeMap.MatchNamesWithUnderscores = true;
SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddTransient<DatabaseInitializer>();

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IReceptionistRepository, ReceptionistRepository>();
builder.Services.AddScoped<IOfficeReplicaRepository, OfficeReplicaRepository>();
builder.Services.AddScoped<IDoctorPatientAccessRepository, DoctorPatientAccessRepository>();
builder.Services.AddScoped<ISpecializationServiceReplicaRepository, SpecializationServiceReplicaRepository>();
builder.Services.AddScoped<IRegisteredEmailReplicaRepository, RegisteredEmailReplicaRepository>();

builder.Services.AddScoped<IProfileEventPublisher, MassTransitProfileEventPublisher>();

builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IReceptionistService, ReceptionistService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OfficeEventsConsumer>();
    x.AddConsumer<SpecializationServiceEventsConsumer>();
    x.AddConsumer<AppointmentStatusChangedConsumer>();
    x.AddConsumer<UserEmailEventsConsumer>();

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

        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await initializer.InitializeAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to initialize PostgreSQL schema. Verify that PostgreSQL server is up and reachable.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
