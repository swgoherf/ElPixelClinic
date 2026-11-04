using Dapper;
using Microsoft.Extensions.Logging;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IDbConnectionFactory connectionFactory, ILogger<DatabaseInitializer> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing PostgreSQL database schema for Profiles API...");

        DefaultTypeMap.MatchNamesWithUnderscores = true;
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            CREATE TABLE IF NOT EXISTS doctors (
                id UUID PRIMARY KEY,
                photo_url TEXT NULL,
                first_name VARCHAR(100) NOT NULL,
                last_name VARCHAR(100) NOT NULL,
                middle_name VARCHAR(100) NULL,
                date_of_birth DATE NOT NULL,
                email VARCHAR(255) NOT NULL UNIQUE,
                specialization VARCHAR(100) NOT NULL,
                office_id UUID NOT NULL,
                career_start_year INT NOT NULL,
                status VARCHAR(50) NOT NULL,
                created_at TIMESTAMPTZ NOT NULL,
                updated_at TIMESTAMPTZ NOT NULL
            );

            CREATE INDEX IF NOT EXISTS ix_doctors_status ON doctors(status);
            CREATE INDEX IF NOT EXISTS ix_doctors_specialization ON doctors(specialization);
            CREATE INDEX IF NOT EXISTS ix_doctors_office_id ON doctors(office_id);
            CREATE INDEX IF NOT EXISTS ix_doctors_names ON doctors(last_name, first_name);

            CREATE TABLE IF NOT EXISTS patients (
                id UUID PRIMARY KEY,
                photo_url TEXT NULL,
                first_name VARCHAR(100) NOT NULL,
                last_name VARCHAR(100) NOT NULL,
                middle_name VARCHAR(100) NULL,
                phone_number VARCHAR(50) NULL,
                date_of_birth DATE NOT NULL,
                is_linked_to_account BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMPTZ NOT NULL,
                updated_at TIMESTAMPTZ NOT NULL
            );

            CREATE INDEX IF NOT EXISTS ix_patients_is_linked ON patients(is_linked_to_account);
            CREATE INDEX IF NOT EXISTS ix_patients_names ON patients(last_name, first_name);

            CREATE TABLE IF NOT EXISTS receptionists (
                id UUID PRIMARY KEY,
                photo_url TEXT NULL,
                first_name VARCHAR(100) NOT NULL,
                last_name VARCHAR(100) NOT NULL,
                middle_name VARCHAR(100) NULL,
                email VARCHAR(255) NOT NULL UNIQUE,
                office_id UUID NOT NULL,
                created_at TIMESTAMPTZ NOT NULL,
                updated_at TIMESTAMPTZ NOT NULL
            );

            CREATE INDEX IF NOT EXISTS ix_receptionists_office_id ON receptionists(office_id);

            CREATE TABLE IF NOT EXISTS offices_replica (
                id UUID PRIMARY KEY,
                address TEXT NOT NULL,
                is_active BOOLEAN NOT NULL DEFAULT TRUE
            );

            CREATE TABLE IF NOT EXISTS specialization_services_replica (
                id UUID PRIMARY KEY,
                specialization_name VARCHAR(100) NOT NULL,
                service_name VARCHAR(200) NOT NULL
            );

            CREATE INDEX IF NOT EXISTS ix_spec_services_spec ON specialization_services_replica(specialization_name);

            CREATE TABLE IF NOT EXISTS doctor_patient_access_replica (
                appointment_id UUID PRIMARY KEY,
                doctor_id UUID NOT NULL,
                patient_id UUID NOT NULL
            );

            CREATE INDEX IF NOT EXISTS ix_doctor_patient_access_pair ON doctor_patient_access_replica(doctor_id, patient_id);
            CREATE INDEX IF NOT EXISTS ix_doctor_patient_access_patient ON doctor_patient_access_replica(patient_id);

            CREATE TABLE IF NOT EXISTS registered_emails_replica (
                email VARCHAR(255) PRIMARY KEY,
                user_id UUID NOT NULL,
                created_at TIMESTAMPTZ NOT NULL
            );

            CREATE INDEX IF NOT EXISTS ix_registered_emails_lower ON registered_emails_replica(LOWER(email));
        """;

        await connection.ExecuteAsync(sql);
        _logger.LogInformation("PostgreSQL schema for Profiles API initialized successfully.");
    }
}
