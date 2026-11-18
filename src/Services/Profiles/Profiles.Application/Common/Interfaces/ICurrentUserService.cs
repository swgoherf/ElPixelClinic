namespace Profiles.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    bool IsReceptionistOrAdmin { get; }
    bool IsDoctor { get; }
    bool IsPatient { get; }
    Guid GetRequiredUserId();
}
