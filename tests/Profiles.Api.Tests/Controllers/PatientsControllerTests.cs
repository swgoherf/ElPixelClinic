using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Profiles.Api.Controllers;
using Profiles.Application.DTOs.PatientDtos;
using Profiles.Application.Services;

namespace Profiles.Api.Tests.Controllers;

public class PatientsControllerTests
{
    private readonly Mock<IPatientService> _patientServiceMock;
    private readonly PatientsController _controller;

    public PatientsControllerTests()
    {
        _patientServiceMock = new Mock<IPatientService>();
        _controller = new PatientsController(_patientServiceMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Alice Smith")]
    public async Task GetPatients_ShouldCallService_AndReturnOkResult(string? fullName)
    {
        // Arrange
        var expectedPatients = new List<PatientListItemDto>
        {
            new(Guid.NewGuid(), "Alice Smith", "+1234567890")
        };

        _patientServiceMock
            .Setup(s => s.GetListAsync(fullName))
            .ReturnsAsync(expectedPatients);

        // Act
        var result = await _controller.GetPatients(fullName);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedPatients);
        _patientServiceMock.Verify(s => s.GetListAsync(fullName), Times.Once);
    }

    [Fact]
    public async Task GetPatientById_ShouldCallService_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var personalInfo = new PatientPersonalInfoDto(
            id,
            "photo.jpg",
            "Alice",
            "Smith",
            null,
            "Alice Smith",
            "+1234567890",
            new DateOnly(1990, 1, 1),
            true
        );
        var expectedProfile = new PatientFullProfileDto(personalInfo, null);

        _patientServiceMock
            .Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _controller.GetPatientById(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedProfile);
    }

    [Fact]
    public async Task GetPatientPersonalInfo_ShouldCallService_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedPersonalInfo = new PatientPersonalInfoDto(
            id,
            "photo.jpg",
            "Alice",
            "Smith",
            null,
            "Alice Smith",
            "+1234567890",
            new DateOnly(1990, 1, 1),
            true
        );

        _patientServiceMock
            .Setup(s => s.GetPersonalInfoAsync(id))
            .ReturnsAsync(expectedPersonalInfo);

        // Act
        var result = await _controller.GetPatientPersonalInfo(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedPersonalInfo);
    }

    [Fact]
    public async Task GetPatientAppointmentResults_ShouldCallService_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedResults = new List<PatientAppointmentResultDto>
        {
            new(
                Guid.NewGuid(),
                DateTime.UtcNow,
                "Dr. John Doe",
                "Cardiology",
                "Drink water",
                "Healthy"
            )
        };

        _patientServiceMock
            .Setup(s => s.GetAppointmentResultsAsync(id))
            .ReturnsAsync(expectedResults);

        // Act
        var result = await _controller.GetPatientAppointmentResults(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedResults);
    }

    [Fact]
    public async Task CreateSelf_WhenHasMatchIsTrue_ShouldReturnOkResult()
    {
        // Arrange
        var dto = new CreatePatientSelfDto("photo.jpg", "Alice", "Smith", null, "+1234567890", new DateOnly(1990, 1, 1));
        var matchedProfile = new PatientPersonalInfoDto(
            Guid.NewGuid(),
            "photo.jpg",
            "Alice",
            "Smith",
            null,
            "Alice Smith",
            "+1234567890",
            new DateOnly(1990, 1, 1),
            false
        );
        var matchResult = new PatientMatchCheckResultDto(
            HasMatch: true,
            ExistingProfileId: matchedProfile.Id,
            MatchedProfile: matchedProfile,
            CreatedProfile: null
        );

        _patientServiceMock
            .Setup(s => s.CreateSelfAsync(dto))
            .ReturnsAsync(matchResult);

        // Act
        var result = await _controller.CreateSelf(dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(matchResult);
    }

    [Fact]
    public async Task CreateSelf_WhenHasMatchIsFalse_ShouldReturnCreatedAtActionResult()
    {
        // Arrange
        var dto = new CreatePatientSelfDto("photo.jpg", "Alice", "Smith", null, "+1234567890", new DateOnly(1990, 1, 1));
        var createdProfile = new PatientPersonalInfoDto(
            Guid.NewGuid(),
            "photo.jpg",
            "Alice",
            "Smith",
            null,
            "Alice Smith",
            "+1234567890",
            new DateOnly(1990, 1, 1),
            true
        );
        var matchResult = new PatientMatchCheckResultDto(
            HasMatch: false,
            ExistingProfileId: null,
            MatchedProfile: null,
            CreatedProfile: createdProfile
        );

        _patientServiceMock
            .Setup(s => s.CreateSelfAsync(dto))
            .ReturnsAsync(matchResult);

        // Act
        var result = await _controller.CreateSelf(dto);

        // Assert
        var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.ActionName.Should().Be(nameof(PatientsController.GetPatientPersonalInfo));
        createdAtResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(createdProfile.Id);
        createdAtResult.Value.Should().BeEquivalentTo(matchResult);
    }

    [Fact]
    public async Task ForceCreateSelf_ShouldCallService_AndReturnCreatedAtActionResult()
    {
        // Arrange
        var dto = new CreatePatientSelfDto("photo.jpg", "Alice", "Smith", null, "+1234567890", new DateOnly(1990, 1, 1));
        var createdProfile = new PatientPersonalInfoDto(
            Guid.NewGuid(),
            "photo.jpg",
            "Alice",
            "Smith",
            null,
            "Alice Smith",
            "+1234567890",
            new DateOnly(1990, 1, 1),
            true
        );

        _patientServiceMock
            .Setup(s => s.ForceCreateSelfAsync(dto))
            .ReturnsAsync(createdProfile);

        // Act
        var result = await _controller.ForceCreateSelf(dto);

        // Assert
        var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.ActionName.Should().Be(nameof(PatientsController.GetPatientPersonalInfo));
        createdAtResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(createdProfile.Id);
        createdAtResult.Value.Should().BeEquivalentTo(createdProfile);
    }

    [Fact]
    public async Task ConfirmMatch_ShouldCallService_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updatedProfile = new PatientPersonalInfoDto(
            id,
            "photo.jpg",
            "Alice",
            "Smith",
            null,
            "Alice Smith",
            "+1234567890",
            new DateOnly(1990, 1, 1),
            true
        );

        _patientServiceMock
            .Setup(s => s.ConfirmMatchAsync(id))
            .ReturnsAsync(updatedProfile);

        // Act
        var result = await _controller.ConfirmMatch(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(updatedProfile);
    }

    [Fact]
    public async Task CreateByAdmin_ShouldCallService_AndReturnCreatedAtActionResult()
    {
        // Arrange
        var dto = new CreatePatientByAdminDto("Bob", "Brown", "Middle", new DateOnly(1985, 3, 15));
        var createdProfile = new PatientPersonalInfoDto(
            Guid.NewGuid(),
            null,
            dto.FirstName,
            dto.LastName,
            dto.MiddleName,
            "Bob Middle Brown",
            null,
            dto.DateOfBirth,
            false
        );

        _patientServiceMock
            .Setup(s => s.CreateByAdminAsync(dto))
            .ReturnsAsync(createdProfile);

        // Act
        var result = await _controller.CreateByAdmin(dto);

        // Assert
        var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.ActionName.Should().Be(nameof(PatientsController.GetPatientPersonalInfo));
        createdAtResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(createdProfile.Id);
        createdAtResult.Value.Should().BeEquivalentTo(createdProfile);
    }

    [Fact]
    public async Task UpdatePatient_ShouldCallService_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdatePatientDto("new_photo.jpg", "Alice", "Johnson", null, "+9876543210", new DateOnly(1990, 1, 1));
        var updatedProfile = new PatientPersonalInfoDto(
            id,
            dto.PhotoUrl,
            dto.FirstName,
            dto.LastName,
            dto.MiddleName,
            "Alice Johnson",
            dto.PhoneNumber,
            dto.DateOfBirth,
            true
        );

        _patientServiceMock
            .Setup(s => s.UpdateAsync(id, dto))
            .ReturnsAsync(updatedProfile);

        // Act
        var result = await _controller.UpdatePatient(id, dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(updatedProfile);
    }

    [Fact]
    public async Task DeletePatient_ShouldCallService_AndReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _patientServiceMock
            .Setup(s => s.DeleteAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeletePatient(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _patientServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
    }
}
