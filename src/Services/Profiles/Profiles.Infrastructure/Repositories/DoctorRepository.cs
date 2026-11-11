using System.Text;
using Dapper;
using Profiles.Application.Common.Interfaces;
using Profiles.Application.DTOs.DoctorDtos;
using Profiles.Domain.Entities;
using Profiles.Domain.Enums;

namespace Profiles.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DoctorRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<DoctorEntity>> GetAllAsync(string? fullName = null, string? specialization = null, Guid? officeId = null, bool atWorkOnly = false)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sqlBuilder = new StringBuilder("SELECT * FROM doctors WHERE 1=1 ");
        var parameters = new DynamicParameters();

        if (atWorkOnly)
        {
            sqlBuilder.Append("AND status = @AtWorkStatus ");
            parameters.Add("AtWorkStatus", nameof(DoctorStatus.AtWork));
        }

        if (!string.IsNullOrWhiteSpace(specialization))
        {
            sqlBuilder.Append("AND LOWER(specialization) = LOWER(@Specialization) ");
            parameters.Add("Specialization", specialization.Trim());
        }

        if (officeId is { } id && id != Guid.Empty)
        {
            sqlBuilder.Append("AND office_id = @OfficeId ");
            parameters.Add("OfficeId", id);
        }

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            sqlBuilder.Append(Data.SqlSearchFilters.FullNameMatchSql);
            parameters.Add("FullNamePattern", $"%{fullName.Trim()}%");
        }

        sqlBuilder.Append("ORDER BY last_name, first_name");

        var rows = await connection.QueryAsync<DoctorEntityDbModel>(sqlBuilder.ToString(), parameters);

        return rows.Select(x => x.ToEntity());
    }

    public async Task<DoctorEntity?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT * FROM doctors WHERE id = @Id";

        var row = await connection.QuerySingleOrDefaultAsync<DoctorEntityDbModel>(sql, new { Id = id });

        return row?.ToEntity();
    }

    public async Task<DoctorEntity?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT * FROM doctors WHERE LOWER(email) = LOWER(@Email)";

        var row = await connection.QuerySingleOrDefaultAsync<DoctorEntityDbModel>(sql, new { Email = email.Trim() });

        return row?.ToEntity();
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeDoctorId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT COUNT(1) FROM doctors 
            WHERE LOWER(email) = LOWER(@Email) 
              AND (@ExcludeDoctorId IS NULL OR id != @ExcludeDoctorId)
        """;

        var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email.Trim(), ExcludeDoctorId = excludeDoctorId });

        return count > 0;
    }

    public async Task AddAsync(DoctorEntity doctor)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO doctors (
                id, photo_url, first_name, last_name, middle_name,
                date_of_birth, email, specialization, office_id,
                career_start_year, status, created_at, updated_at
            ) VALUES (
                @Id, @PhotoUrl, @FirstName, @LastName, @MiddleName,
                @DateOfBirth, @Email, @Specialization, @OfficeId,
                @CareerStartYear, @Status, @CreatedAt, @UpdatedAt
            )
        """;

        await connection.ExecuteAsync(sql, new
        {
            doctor.Id,
            doctor.PhotoUrl,
            doctor.FirstName,
            doctor.LastName,
            doctor.MiddleName,
            DateOfBirth = doctor.DateOfBirth.ToDateTime(TimeOnly.MinValue),
            doctor.Email,
            doctor.Specialization,
            doctor.OfficeId,
            doctor.CareerStartYear,
            Status = doctor.Status.ToString(),
            doctor.CreatedAt,
            doctor.UpdatedAt
        });
    }

    public async Task UpdateAsync(DoctorEntity doctor)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE doctors SET
                photo_url = @PhotoUrl,
                first_name = @FirstName,
                last_name = @LastName,
                middle_name = @MiddleName,
                date_of_birth = @DateOfBirth,
                email = @Email,
                specialization = @Specialization,
                office_id = @OfficeId,
                career_start_year = @CareerStartYear,
                updated_at = @UpdatedAt
            WHERE id = @Id
        """;

        await connection.ExecuteAsync(sql, new
        {
            doctor.Id,
            doctor.PhotoUrl,
            doctor.FirstName,
            doctor.LastName,
            doctor.MiddleName,
            DateOfBirth = doctor.DateOfBirth.ToDateTime(TimeOnly.MinValue),
            doctor.Email,
            doctor.Specialization,
            doctor.OfficeId,
            doctor.CareerStartYear,
            doctor.UpdatedAt
        });
    }

    public async Task UpdateStatusAsync(Guid id, DoctorStatus status)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE doctors SET
                status = @Status,
                updated_at = @UpdatedAt
            WHERE id = @Id
        """;

        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            Status = status.ToString(),
            UpdatedAt = DateTime.UtcNow
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "DELETE FROM doctors WHERE id = @Id";

        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<IEnumerable<DoctorOfficeFilterItemDto>> GetOfficesBySpecializationAsync(string? specialization = null, bool atWorkOnly = false)
    {
        using var connection = _connectionFactory.CreateConnection();

        if (string.IsNullOrWhiteSpace(specialization) && !atWorkOnly)
        {
            const string allOfficesSql = """
                SELECT id AS Id, address AS Address
                FROM offices_replica
                WHERE is_active = TRUE
                ORDER BY address
            """;

            return await connection.QueryAsync<DoctorOfficeFilterItemDto>(allOfficesSql);
        }

        var sqlBuilder = new StringBuilder("""
            SELECT DISTINCT o.id AS Id, o.address AS Address
            FROM offices_replica o
            JOIN doctors d ON d.office_id = o.id
            WHERE o.is_active = TRUE
        """);

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(specialization))
        {
            sqlBuilder.Append(" AND LOWER(d.specialization) = LOWER(@Specialization)");
            parameters.Add("Specialization", specialization.Trim());
        }

        if (atWorkOnly)
        {
            sqlBuilder.Append(" AND d.status = @AtWorkStatus");
            parameters.Add("AtWorkStatus", nameof(DoctorStatus.AtWork));
        }

        sqlBuilder.Append(" ORDER BY o.address");

        return await connection.QueryAsync<DoctorOfficeFilterItemDto>(sqlBuilder.ToString(), parameters);
    }

    public async Task<IEnumerable<string>> GetSpecializationsByOfficeAsync(Guid? officeId = null, bool atWorkOnly = false)
    {
        using var connection = _connectionFactory.CreateConnection();

        if (atWorkOnly)
        {
            var sqlBuilder = new StringBuilder("""
                SELECT DISTINCT d.specialization
                FROM doctors d
                WHERE d.status = @AtWorkStatus
            """);

            var parameters = new DynamicParameters();
            parameters.Add("AtWorkStatus", nameof(DoctorStatus.AtWork));

            if (officeId is { } id && id != Guid.Empty)
            {
                sqlBuilder.Append(" AND d.office_id = @OfficeId");
                parameters.Add("OfficeId", id);
            }

            sqlBuilder.Append(" ORDER BY d.specialization");

            return await connection.QueryAsync<string>(sqlBuilder.ToString(), parameters);
        }

        if (officeId is { } office && office != Guid.Empty)
        {
            const string sql = """
                SELECT DISTINCT d.specialization
                FROM doctors d
                WHERE d.office_id = @OfficeId
                ORDER BY d.specialization
            """;

            return await connection.QueryAsync<string>(sql, new { OfficeId = office });
        }

        const string allSql = """
            SELECT DISTINCT d.specialization FROM doctors d
            UNION
            SELECT DISTINCT specialization_name AS specialization FROM specialization_services_replica
            ORDER BY specialization
        """;

        return await connection.QueryAsync<string>(allSql);
    }

    private class DoctorEntityDbModel
    {
        public Guid Id { get; set; }
        public string? PhotoUrl { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public Guid OfficeId { get; set; }
        public int CareerStartYear { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DoctorEntity ToEntity()
        {
            return new DoctorEntity
            {
                Id = Id,
                PhotoUrl = PhotoUrl,
                FirstName = FirstName,
                LastName = LastName,
                MiddleName = MiddleName,
                DateOfBirth = DateOnly.FromDateTime(DateOfBirth),
                Email = Email,
                Specialization = Specialization,
                OfficeId = OfficeId,
                CareerStartYear = CareerStartYear,
                Status = Enum.TryParse<DoctorStatus>(Status, true, out var parsedStatus) ? parsedStatus : DoctorStatus.AtWork,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
    }
}
