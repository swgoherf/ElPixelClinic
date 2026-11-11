namespace Profiles.Infrastructure.Data;

public static class SqlSearchFilters
{
    public const string FullNameMatchSql = """
        AND (
            (last_name || ' ' || first_name || ' ' || COALESCE(middle_name, '')) ILIKE @FullNamePattern
            OR
            (first_name || ' ' || last_name || ' ' || COALESCE(middle_name, '')) ILIKE @FullNamePattern
            OR
            (first_name || ' ' || COALESCE(middle_name, '') || ' ' || last_name) ILIKE @FullNamePattern
        ) 
    """;
}
