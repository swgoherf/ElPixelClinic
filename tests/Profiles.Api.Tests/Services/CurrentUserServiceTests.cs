using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Profiles.Api.Services;
using Profiles.Application.Common.Constants;
using Profiles.Application.Common.Exceptions;

namespace Profiles.Api.Tests.Services;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CurrentUserService _service;

    public CurrentUserServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _service = new CurrentUserService(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void Properties_WhenHttpContextIsNull_ShouldReturnDefaultValues()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act & Assert
        _service.IsAuthenticated.Should().BeFalse();
        _service.UserId.Should().BeNull();
        _service.Email.Should().BeNull();
        _service.Role.Should().BeNull();
        _service.IsInRole("Admin").Should().BeFalse();
        _service.IsReceptionistOrAdmin.Should().BeFalse();
        _service.IsDoctor.Should().BeFalse();
        _service.IsPatient.Should().BeFalse();

        var act = () => _service.GetRequiredUserId();
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("User is not authenticated.");
    }

    [Fact]
    public void Properties_WhenUserIsNull_ShouldReturnDefaultValues()
    {
        // Arrange
        var context = new DefaultHttpContext { User = null! };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.IsAuthenticated.Should().BeFalse();
        _service.UserId.Should().BeNull();
        _service.Email.Should().BeNull();
        _service.Role.Should().BeNull();
        _service.IsInRole("Admin").Should().BeFalse();
        _service.IsReceptionistOrAdmin.Should().BeFalse();
        _service.IsDoctor.Should().BeFalse();
        _service.IsPatient.Should().BeFalse();

        var act = () => _service.GetRequiredUserId();
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("User is not authenticated.");
    }

    [Fact]
    public void IsAuthenticated_WhenIdentityIsNotAuthenticated_ShouldReturnFalse()
    {
        // Arrange
        var identity = new ClaimsIdentity(); // IsAuthenticated is false when no auth type given
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.IsAuthenticated.Should().BeFalse();
        var act = () => _service.GetRequiredUserId();
        act.Should().Throw<UnauthorizedException>();
    }

    [Fact]
    public void IsAuthenticated_WhenUserIdentityIsNull_ShouldReturnFalse()
    {
        // Arrange
        var principal = new ClaimsPrincipal(); // ClaimsPrincipal with no identities -> Identity is null
        var context = new DefaultHttpContext { User = principal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.IsAuthenticated.Should().BeFalse();
    }

    [Theory]
    [InlineData(ClaimTypes.NameIdentifier)]
    [InlineData("sub")]
    [InlineData("id")]
    [InlineData("userId")]
    public void UserId_WhenClaimHasValidGuid_ShouldReturnParsedGuid(string claimType)
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(claimType, expectedId.ToString())
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.UserId.Should().Be(expectedId);
        _service.GetRequiredUserId().Should().Be(expectedId);
    }

    [Fact]
    public void UserId_WhenClaimHasInvalidGuid_ShouldReturnNull()
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("sub", "not-a-valid-guid")
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.UserId.Should().BeNull();
        var act = () => _service.GetRequiredUserId();
        act.Should().Throw<UnauthorizedException>();
    }

    [Fact]
    public void UserId_WhenNoIdClaimsExist_ShouldReturnNull()
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("other", "value")
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.UserId.Should().BeNull();
    }

    [Theory]
    [InlineData(ClaimTypes.Email, "doctor@clinic.com")]
    [InlineData("email", "patient@clinic.com")]
    public void Email_WhenClaimPresent_ShouldReturnEmail(string claimType, string email)
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(claimType, email)
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.Email.Should().Be(email);
    }

    [Fact]
    public void Email_WhenNoEmailClaim_ShouldReturnNull()
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("some_other_claim", "val")
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.Email.Should().BeNull();
    }

    [Theory]
    [InlineData(ClaimTypes.Role, UserRoles.Doctor)]
    [InlineData("role", UserRoles.Receptionist)]
    public void Role_WhenClaimPresent_ShouldReturnRole(string claimType, string role)
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(claimType, role)
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.Role.Should().Be(role);
    }

    [Fact]
    public void Role_WhenNoRoleClaim_ShouldReturnNull()
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("some_other_claim", "val")
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.Role.Should().BeNull();
    }

    [Fact]
    public void IsInRole_WhenUserIsInRoleViaPrincipal_ShouldReturnTrue()
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRoles.Admin)
        }, "TestAuth", ClaimTypes.Name, ClaimTypes.Role);
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.IsInRole(UserRoles.Admin).Should().BeTrue();
    }

    [Fact]
    public void IsInRole_WhenRoleClaimCaseDiffers_ShouldReturnTrueCaseInsensitively()
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("role", "doctor")
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.IsInRole("Doctor").Should().BeTrue();
        _service.IsInRole("DOCTOR").Should().BeTrue();
    }

    [Fact]
    public void IsInRole_WhenRoleNotFound_ShouldReturnFalse()
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("role", "Doctor")
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.IsInRole(UserRoles.Admin).Should().BeFalse();
    }

    [Theory]
    [InlineData(UserRoles.Receptionist, true, false, false)]
    [InlineData(UserRoles.Admin, true, false, false)]
    [InlineData(UserRoles.Doctor, false, true, false)]
    [InlineData(UserRoles.Patient, false, false, true)]
    [InlineData("OtherRole", false, false, false)]
    public void RoleHelpers_ShouldReturnExpectedFlags(
        string role,
        bool expectedReceptionistOrAdmin,
        bool expectedDoctor,
        bool expectedPatient)
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, role)
        }, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act & Assert
        _service.IsReceptionistOrAdmin.Should().Be(expectedReceptionistOrAdmin);
        _service.IsDoctor.Should().Be(expectedDoctor);
        _service.IsPatient.Should().Be(expectedPatient);
    }
}
