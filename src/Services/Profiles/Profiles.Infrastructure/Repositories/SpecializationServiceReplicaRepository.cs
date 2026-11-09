using Dapper;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Infrastructure.Repositories;

public class SpecializationServiceReplicaRepository : ISpecializationServiceReplicaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SpecializationServiceReplicaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<string>> GetServicesBySpecializationAsync(string specializationName)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT service_name FROM specialization_services_replica
            WHERE LOWER(specialization_name) = LOWER(@SpecializationName)
            ORDER BY service_name
        """;

        return await connection.QueryAsync<string>(sql, new { SpecializationName = specializationName.Trim() });
    }

    public async Task<bool> SpecializationExistsAsync(string specializationName)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT COUNT(1) FROM specialization_services_replica
            WHERE LOWER(specialization_name) = LOWER(@SpecializationName)
        """;

        var count = await connection.ExecuteScalarAsync<int>(sql, new { SpecializationName = specializationName.Trim() });

        return count > 0;
    }

    public async Task<bool> HasAnyAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT COUNT(1) FROM specialization_services_replica";

        var count = await connection.ExecuteScalarAsync<int>(sql);

        return count > 0;
    }

    public async Task AddAsync(string specializationName, string serviceName)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO specialization_services_replica (id, specialization_name, service_name)
            VALUES (@Id, @SpecializationName, @ServiceName)
        """;

        await connection.ExecuteAsync(sql, new
        {
            Id = Guid.NewGuid(),
            SpecializationName = specializationName.Trim(),
            ServiceName = serviceName.Trim()
        });
    }

    public async Task DeleteAsync(string specializationName, string serviceName)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            DELETE FROM specialization_services_replica
            WHERE LOWER(specialization_name) = LOWER(@SpecializationName)
              AND LOWER(service_name) = LOWER(@ServiceName)
        """;

        await connection.ExecuteAsync(sql, new
        {
            SpecializationName = specializationName.Trim(),
            ServiceName = serviceName.Trim()
        });
    }
}
