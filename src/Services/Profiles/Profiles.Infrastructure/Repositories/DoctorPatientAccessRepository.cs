using Dapper;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Infrastructure.Repositories;

public class DoctorPatientAccessRepository : IDoctorPatientAccessRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DoctorPatientAccessRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> HasAccessAsync(Guid doctorId, Guid patientId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT COUNT(1) FROM doctor_patient_access_replica
            WHERE doctor_id = @DoctorId AND patient_id = @PatientId
        """;

        var count = await connection.ExecuteScalarAsync<int>(sql, new { DoctorId = doctorId, PatientId = patientId });

        return count > 0;
    }

    public async Task GrantAccessAsync(Guid appointmentId, Guid doctorId, Guid patientId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO doctor_patient_access_replica (appointment_id, doctor_id, patient_id)
            VALUES (@AppointmentId, @DoctorId, @PatientId)
            ON CONFLICT (appointment_id) DO UPDATE SET
                doctor_id = EXCLUDED.doctor_id,
                patient_id = EXCLUDED.patient_id;
        """;

        await connection.ExecuteAsync(sql, new
        {
            AppointmentId = appointmentId,
            DoctorId = doctorId,
            PatientId = patientId
        });
    }

    public async Task RevokeAccessAsync(Guid appointmentId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            DELETE FROM doctor_patient_access_replica
            WHERE appointment_id = @AppointmentId
        """;

        await connection.ExecuteAsync(sql, new { AppointmentId = appointmentId });
    }
}
