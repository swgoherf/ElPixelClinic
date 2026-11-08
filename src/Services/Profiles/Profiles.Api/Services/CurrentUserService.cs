using System.Security.Claims;
using Profiles.Application.Common.Constants;
using Profiles.Application.Common.Exceptions;
using Profiles.Application.Common.Interfaces;

namespace Profiles.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var rawId = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User?.FindFirstValue("sub")
                     ?? User?.FindFirstValue("id")
                     ?? User?.FindFirstValue("userId");

            return Guid.TryParse(rawId, out var parsed) ? parsed : null;
        }
    }

    public string? Email =>
        User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue("email");

    public string? Role =>
        User?.FindFirstValue(ClaimTypes.Role)
        ?? User?.FindFirstValue("role");

    public bool IsInRole(string role)
    {
        if (User is null) return false;

        if (User.IsInRole(role)) return true;

        var roles = User.FindAll(ClaimTypes.Role)
            .Concat(User.FindAll("role"))
            .Select(c => c.Value);

        return roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsReceptionistOrAdmin =>
        IsInRole(UserRoles.Receptionist) || IsInRole(UserRoles.Admin);

    public bool IsDoctor =>
        IsInRole(UserRoles.Doctor);

    public bool IsPatient =>
        IsInRole(UserRoles.Patient);

    public Guid GetRequiredUserId()
    {
        if (!IsAuthenticated || UserId is null)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        return UserId.Value;
    }
}
