using Dapper;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Infrastructure.Repositories;

public class RegisteredEmailReplicaRepository : IRegisteredEmailReplicaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RegisteredEmailReplicaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = """
            SELECT COUNT(1) FROM registered_emails_replica
            WHERE LOWER(email) = LOWER(@Email)
        """;
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email.Trim() });
        return count > 0;
    }

    public async Task UpsertAsync(string email, Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = """
            INSERT INTO registered_emails_replica (email, user_id, created_at)
            VALUES (@Email, @UserId, @CreatedAt)
            ON CONFLICT (email) DO UPDATE SET
                user_id = EXCLUDED.user_id,
                created_at = EXCLUDED.created_at;
        """;
        await connection.ExecuteAsync(sql, new
        {
            Email = email.Trim().ToLowerInvariant(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task DeleteAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "DELETE FROM registered_emails_replica WHERE LOWER(email) = LOWER(@Email)";
        await connection.ExecuteAsync(sql, new { Email = email.Trim() });
    }
}
