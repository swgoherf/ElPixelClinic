using FluentAssertions;
using Moq;
using Profiles.Application.Common.Exceptions;
using Profiles.Application.Common.Interfaces;
using Profiles.Application.DTOs.PatientDtos;
using Profiles.Application.Services;
using Profiles.Domain.Entities;

namespace Profiles.Api.Tests.Services;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IDoctorPatientAccessRepository> _accessRepositoryMock;
    private readonly Mock<IProfileEventPublisher> _eventPublisherMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _accessRepositoryMock = new Mock<IDoctorPatientAccessRepository>();
        _eventPublisherMock = new Mock<IProfileEventPublisher>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _patientService = new PatientService(
            _patientRepositoryMock.Object,
            _accessRepositoryMock.Object,
            _eventPublisherMock.Object,
            _currentUserServiceMock.Object
        );
    }

    [Fact]
    public async Task CreateSelfAsync_WhenMatchFoundWithScoreAbove13_ShouldReturnMatchedProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
        _currentUserServiceMock.Setup(s => s.IsAuthenticated).Returns(true);

        var dto = new CreatePatientSelfDto(
            PhotoUrl: null,
            FirstName: "Ivan",
            LastName: "Ivanov",
            MiddleName: "Ivanovich",
            PhoneNumber: "+1234567890",
            DateOfBirth: new DateOnly(1990, 5, 20)
        );

        var candidate = new PatientEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Ivan",
            LastName = "Ivanov",
            MiddleName = "Ivanovich",
            DateOfBirth = new DateOnly(1990, 5, 20),
            IsLinkedToAccount = false
        };

        _patientRepositoryMock
            .Setup(r => r.FindUnlinkedCandidatesAsync("Ivan", "Ivanov", new DateOnly(1990, 5, 20)))
            .ReturnsAsync(new List<PatientEntity> { candidate });

        // Act
        var result = await _patientService.CreateSelfAsync(dto);

        // Assert
        result.HasMatch.Should().BeTrue();
        result.ExistingProfileId.Should().Be(candidate.Id);
        result.CreatedProfile.Should().BeNull();
        _patientRepositoryMock.Verify(r => r.AddAsync(It.IsAny<PatientEntity>()), Times.Never);
    }

    [Fact]
    public async Task CreateSelfAsync_WhenNoMatchFound_ShouldCreateNewProfileWithAccountId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
        _currentUserServiceMock.Setup(s => s.IsAuthenticated).Returns(true);

        var dto = new CreatePatientSelfDto(
            PhotoUrl: null,
            FirstName: "Petr",
            LastName: "Petrov",
            MiddleName: null,
            PhoneNumber: "+1234567890",
            DateOfBirth: new DateOnly(1985, 1, 10)
        );

        _patientRepositoryMock
            .Setup(r => r.FindUnlinkedCandidatesAsync("Petr", "Petrov", new DateOnly(1985, 1, 10)))
            .ReturnsAsync(new List<PatientEntity>());

        PatientEntity? capturedEntity = null;
        _patientRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<PatientEntity>()))
            .Callback<PatientEntity>(e => capturedEntity = e)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _patientService.CreateSelfAsync(dto);

        // Assert
        result.HasMatch.Should().BeFalse();
        result.CreatedProfile.Should().NotBeNull();
        capturedEntity.Should().NotBeNull();
        capturedEntity!.AccountId.Should().Be(userId);
        capturedEntity.IsLinkedToAccount.Should().BeTrue();
    }

    [Fact]
    public async Task ConfirmMatchAsync_WhenProfileExists_ShouldLinkToCurrentUserId()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.GetRequiredUserId()).Returns(currentUserId);

        var existingPatientId = Guid.NewGuid();
        var candidate = new PatientEntity
        {
            Id = existingPatientId,
            FirstName = "Ivan",
            LastName = "Ivanov",
            DateOfBirth = new DateOnly(1990, 5, 20),
            IsLinkedToAccount = false,
            AccountId = null
        };

        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(existingPatientId))
            .ReturnsAsync(candidate);

        // Act
        var result = await _patientService.ConfirmMatchAsync(existingPatientId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingPatientId);
        result.IsLinkedToAccount.Should().BeTrue();
        candidate.AccountId.Should().Be(currentUserId);
        _patientRepositoryMock.Verify(r => r.LinkToAccountAsync(existingPatientId, currentUserId), Times.Once);
    }

    [Fact]
    public async Task ConfirmMatchAsync_WhenProfileAlreadyLinkedToAnotherUser_ShouldThrowConflictException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.GetRequiredUserId()).Returns(currentUserId);

        var existingPatientId = Guid.NewGuid();
        var candidate = new PatientEntity
        {
            Id = existingPatientId,
            FirstName = "Ivan",
            LastName = "Ivanov",
            DateOfBirth = new DateOnly(1990, 5, 20),
            IsLinkedToAccount = true,
            AccountId = anotherUserId
        };

        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(existingPatientId))
            .ReturnsAsync(candidate);

        // Act
        var act = () => _patientService.ConfirmMatchAsync(existingPatientId);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Patient profile is already linked to another account.");
    }

    [Fact]
    public async Task GetPersonalInfoAsync_WhenPatientIsOwnerByAccountId_ShouldAllowAccess()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _currentUserServiceMock.Setup(s => s.IsAuthenticated).Returns(true);
        _currentUserServiceMock.Setup(s => s.IsPatient).Returns(true);
        _currentUserServiceMock.Setup(s => s.UserId).Returns(currentUserId);

        var patient = new PatientEntity
        {
            Id = patientId,
            AccountId = currentUserId,
            FirstName = "Anna",
            LastName = "Karenina",
            DateOfBirth = new DateOnly(1992, 3, 15),
            IsLinkedToAccount = true
        };

        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetPersonalInfoAsync(patientId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(patientId);
    }

    [Fact]
    public async Task GetPersonalInfoAsync_WhenPatientIsNotOwner_ShouldThrowForbiddenException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _currentUserServiceMock.Setup(s => s.IsAuthenticated).Returns(true);
        _currentUserServiceMock.Setup(s => s.IsPatient).Returns(true);
        _currentUserServiceMock.Setup(s => s.UserId).Returns(currentUserId);

        var patient = new PatientEntity
        {
            Id = patientId,
            AccountId = anotherUserId,
            FirstName = "Anna",
            LastName = "Karenina",
            DateOfBirth = new DateOnly(1992, 3, 15),
            IsLinkedToAccount = true
        };

        _patientRepositoryMock
            .Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(patient);

        // Act
        var act = () => _patientService.GetPersonalInfoAsync(patientId);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Patients are only allowed to view their own profile.");
    }
}
