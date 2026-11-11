using System.Data;
using Dapper;

namespace Profiles.Infrastructure.Data;

public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly Parse(object value)
    {
        return value switch
        {
            DateTime dt => DateOnly.FromDateTime(dt),
            DateOnly d => d,
            string s when DateTime.TryParse(s, out var dt) => DateOnly.FromDateTime(dt),
            _ => throw new InvalidCastException($"Cannot convert {value?.GetType().FullName} to DateOnly")
        };
    }
}
