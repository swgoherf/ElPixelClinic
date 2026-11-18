using Dapper;
using Profiles.Application.Common.Interfaces;
using Profiles.Domain.Entities;

namespace Profiles.Infrastructure.Repositories;

public class ReceptionistRepository : IReceptionistRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReceptionistRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<ReceptionistEntity>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT * FROM receptionists ORDER BY last_name, first_name";

        var rows = await connection.QueryAsync<ReceptionistEntityDbModel>(sql);

        return rows.Select(x => x.ToEntity());
    }

    public async Task<ReceptionistEntity?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT * FROM receptionists WHERE id = @Id";

        var row = await connection.QuerySingleOrDefaultAsync<ReceptionistEntityDbModel>(sql, new { Id = id });

        return row?.ToEntity();
    }

    public async Task<ReceptionistEntity?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT * FROM receptionists WHERE LOWER(email) = LOWER(@Email)";

        var row = await connection.QuerySingleOrDefaultAsync<ReceptionistEntityDbModel>(sql, new { Email = email.Trim() });

        return row?.ToEntity();
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeReceptionistId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT COUNT(1) FROM receptionists 
            WHERE LOWER(email) = LOWER(@Email) 
              AND (@ExcludeId IS NULL OR id != @ExcludeId)
        """;

        var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email.Trim(), ExcludeId = excludeReceptionistId });

        return count > 0;
    }

    public async Task AddAsync(ReceptionistEntity receptionist)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO receptionists (
                id, photo_url, first_name, last_name, middle_name,
                email, office_id, created_at, updated_at
            ) VALUES (
                @Id, @PhotoUrl, @FirstName, @LastName, @MiddleName,
                @Email, @OfficeId, @CreatedAt, @UpdatedAt
            )
        """;

        await connection.ExecuteAsync(sql, new
        {
            receptionist.Id,
            receptionist.PhotoUrl,
            receptionist.FirstName,
            receptionist.LastName,
            receptionist.MiddleName,
            receptionist.Email,
            receptionist.OfficeId,
            receptionist.CreatedAt,
            receptionist.UpdatedAt
        });
    }

    public async Task UpdateAsync(ReceptionistEntity receptionist)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE receptionists SET
                photo_url = @PhotoUrl,
                first_name = @FirstName,
                last_name = @LastName,
                middle_name = @MiddleName,
                office_id = @OfficeId,
                updated_at = @UpdatedAt
            WHERE id = @Id
        """;

        await connection.ExecuteAsync(sql, new
        {
            receptionist.Id,
            receptionist.PhotoUrl,
            receptionist.FirstName,
            receptionist.LastName,
            receptionist.MiddleName,
            receptionist.OfficeId,
            receptionist.UpdatedAt
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "DELETE FROM receptionists WHERE id = @Id";

        await connection.ExecuteAsync(sql, new { Id = id });
    }

    private class ReceptionistEntityDbModel
    {
        public Guid Id { get; set; }
        public string? PhotoUrl { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = string.Empty;
        public Guid OfficeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ReceptionistEntity ToEntity()
        {
            return new ReceptionistEntity
            {
                Id = Id,
                PhotoUrl = PhotoUrl,
                FirstName = FirstName,
                LastName = LastName,
                MiddleName = MiddleName,
                Email = Email,
                OfficeId = OfficeId,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
    }
}
