using System.Text;
using Dapper;
using Profiles.Application.Common.Interfaces;
using Profiles.Domain.Entities;

namespace Profiles.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PatientRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<PatientEntity>> GetAllAsync(string? fullName = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sqlBuilder = new StringBuilder("SELECT * FROM patients WHERE 1=1 ");
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            sqlBuilder.Append(Data.SqlSearchFilters.FullNameMatchSql);
            parameters.Add("FullNamePattern", $"%{fullName.Trim()}%");
        }

        sqlBuilder.Append("ORDER BY last_name, first_name");

        var rows = await connection.QueryAsync<PatientEntityDbModel>(sqlBuilder.ToString(), parameters);

        return rows.Select(x => x.ToEntity());
    }

    public async Task<IEnumerable<PatientEntity>> GetUnlinkedAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT * FROM patients WHERE is_linked_to_account = FALSE";

        var rows = await connection.QueryAsync<PatientEntityDbModel>(sql);

        return rows.Select(x => x.ToEntity());
    }

    public async Task<PatientEntity?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT * FROM patients WHERE id = @Id";

        var row = await connection.QuerySingleOrDefaultAsync<PatientEntityDbModel>(sql, new { Id = id });

        return row?.ToEntity();
    }

    public async Task AddAsync(PatientEntity patient)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO patients (
                id, photo_url, first_name, last_name, middle_name,
                phone_number, date_of_birth, is_linked_to_account,
                created_at, updated_at
            ) VALUES (
                @Id, @PhotoUrl, @FirstName, @LastName, @MiddleName,
                @PhoneNumber, @DateOfBirth, @IsLinkedToAccount,
                @CreatedAt, @UpdatedAt
            )
        """;

        await connection.ExecuteAsync(sql, new
        {
            patient.Id,
            patient.PhotoUrl,
            patient.FirstName,
            patient.LastName,
            patient.MiddleName,
            patient.PhoneNumber,
            DateOfBirth = patient.DateOfBirth.ToDateTime(TimeOnly.MinValue),
            patient.IsLinkedToAccount,
            patient.CreatedAt,
            patient.UpdatedAt
        });
    }

    public async Task UpdateAsync(PatientEntity patient)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE patients SET
                photo_url = @PhotoUrl,
                first_name = @FirstName,
                last_name = @LastName,
                middle_name = @MiddleName,
                phone_number = @PhoneNumber,
                date_of_birth = @DateOfBirth,
                updated_at = @UpdatedAt
            WHERE id = @Id
        """;

        await connection.ExecuteAsync(sql, new
        {
            patient.Id,
            patient.PhotoUrl,
            patient.FirstName,
            patient.LastName,
            patient.MiddleName,
            patient.PhoneNumber,
            DateOfBirth = patient.DateOfBirth.ToDateTime(TimeOnly.MinValue),
            patient.UpdatedAt
        });
    }

    public async Task MarkAsLinkedAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE patients SET
                is_linked_to_account = TRUE,
                updated_at = @UpdatedAt
            WHERE id = @Id
        """;

        await connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        using var transaction = connection.BeginTransaction();

        try
        {
            const string deleteAccessSql = "DELETE FROM doctor_patient_access_replica WHERE patient_id = @Id";

            await connection.ExecuteAsync(deleteAccessSql, new { Id = id }, transaction);

            const string deletePatientSql = "DELETE FROM patients WHERE id = @Id";

            await connection.ExecuteAsync(deletePatientSql, new { Id = id }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private class PatientEntityDbModel
    {
        public Guid Id { get; set; }
        public string? PhotoUrl { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsLinkedToAccount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public PatientEntity ToEntity()
        {
            return new PatientEntity
            {
                Id = Id,
                PhotoUrl = PhotoUrl,
                FirstName = FirstName,
                LastName = LastName,
                MiddleName = MiddleName,
                PhoneNumber = PhoneNumber,
                DateOfBirth = DateOnly.FromDateTime(DateOfBirth),
                IsLinkedToAccount = IsLinkedToAccount,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
    }
}
