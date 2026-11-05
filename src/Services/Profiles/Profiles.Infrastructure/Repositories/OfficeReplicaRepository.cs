using Dapper;
using Profiles.Application.Common.Interfaces;
using Profiles.Domain.Entities;

namespace Profiles.Infrastructure.Repositories;

public class OfficeReplicaRepository : IOfficeReplicaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OfficeReplicaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<OfficeReplicaEntity?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM offices_replica WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<OfficeReplicaEntity>(sql, new { Id = id });
    }

    public async Task<IEnumerable<OfficeReplicaEntity>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM offices_replica";
        return await connection.QueryAsync<OfficeReplicaEntity>(sql);
    }

    public async Task UpsertAsync(OfficeReplicaEntity office)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = """
            INSERT INTO offices_replica (id, address, is_active)
            VALUES (@Id, @Address, @IsActive)
            ON CONFLICT (id) DO UPDATE SET
                address = EXCLUDED.address,
                is_active = EXCLUDED.is_active;
        """;
        await connection.ExecuteAsync(sql, office);
    }

    public async Task UpdateStatusAsync(Guid id, bool isActive)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE offices_replica SET is_active = @IsActive WHERE id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id, IsActive = isActive });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "DELETE FROM offices_replica WHERE id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}
