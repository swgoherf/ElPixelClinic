namespace Profiles.Application.Common.Helpers;

public static class FullNameFormatter
{
    public static string Format(string firstName, string lastName, string? middleName)
    {
        return string.IsNullOrWhiteSpace(middleName)
            ? $"{lastName} {firstName}".Trim()
            : $"{lastName} {firstName} {middleName}".Trim();
    }
}
