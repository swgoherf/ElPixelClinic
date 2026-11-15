using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Profiles.Api.Controllers;
using Profiles.Application.DTOs.DoctorDtos;
using Profiles.Application.Services;
using Profiles.Domain.Enums;

namespace Profiles.Api.Tests.Controllers;

public class DoctorsControllerTests
{
    private readonly Mock<IDoctorService> _doctorServiceMock;
    private readonly DoctorsController _controller;

    public DoctorsControllerTests()
    {
        _doctorServiceMock = new Mock<IDoctorService>();
        _controller = new DoctorsController(_doctorServiceMock.Object);
    }

    [Fact]
    public async Task GetDoctors_ShouldPassFiltersWithAtWorkOnlyTrue_AndReturnOkResult()
    {
        // Arrange
        var fullName = "Dr. John";
        var specialization = "Cardiology";
        var officeId = Guid.NewGuid();
        var expectedCards = new List<DoctorCardResponseDto>
        {
            new(
                Guid.NewGuid(),
                "photo.png",
                "John",
                "Doe",
                null,
                "John Doe",
                "Cardiology",
                10,
                officeId,
                "Room 101"
            )
        };

        DoctorFilterParameters? capturedFilters = null;
        _doctorServiceMock
            .Setup(s => s.GetCardsAsync(It.IsAny<DoctorFilterParameters>()))
            .Callback<DoctorFilterParameters>(f => capturedFilters = f)
            .ReturnsAsync(expectedCards);

        // Act
        var result = await _controller.GetDoctors(fullName, specialization, officeId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedCards);

        capturedFilters.Should().NotBeNull();
        capturedFilters!.FullName.Should().Be(fullName);
        capturedFilters.Specialization.Should().Be(specialization);
        capturedFilters.OfficeId.Should().Be(officeId);
        capturedFilters.AtWorkOnly.Should().BeTrue();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetDoctorsForAdmin_ShouldPassFiltersWithGivenAtWorkOnly_AndReturnOkResult(bool atWorkOnly)
    {
        // Arrange
        var fullName = "Dr. Jane";
        var specialization = "Neurology";
        var officeId = Guid.NewGuid();
        var expectedAdminList = new List<DoctorAdminResponseDto>
        {
            new(
                Guid.NewGuid(),
                "photo.jpg",
                "Jane",
                "Doe",
                null,
                "Jane Doe",
                new DateOnly(1985, 5, 20),
                "jane@clinic.com",
                "Neurology",
                officeId,
                "123 Main St",
                2020,
                5,
                DoctorStatus.AtWork
            )
        };

        DoctorFilterParameters? capturedFilters = null;
        _doctorServiceMock
            .Setup(s => s.GetAdminListAsync(It.IsAny<DoctorFilterParameters>()))
            .Callback<DoctorFilterParameters>(f => capturedFilters = f)
            .ReturnsAsync(expectedAdminList);

        // Act
        var result = await _controller.GetDoctorsForAdmin(fullName, specialization, officeId, atWorkOnly);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedAdminList);

        capturedFilters.Should().NotBeNull();
        capturedFilters!.FullName.Should().Be(fullName);
        capturedFilters.Specialization.Should().Be(specialization);
        capturedFilters.OfficeId.Should().Be(officeId);
        capturedFilters.AtWorkOnly.Should().Be(atWorkOnly);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("Therapy", true)]
    public async Task GetOfficesFilter_ShouldCallService_AndReturnOkResult(string? specialization, bool atWorkOnly)
    {
        // Arrange
        var expectedOffices = new List<DoctorOfficeFilterItemDto>
        {
            new(Guid.NewGuid(), "Main Clinic")
        };

        _doctorServiceMock
            .Setup(s => s.GetOfficesFilterAsync(specialization, atWorkOnly))
            .ReturnsAsync(expectedOffices);

        // Act
        var result = await _controller.GetOfficesFilter(specialization, atWorkOnly);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedOffices);
        _doctorServiceMock.Verify(s => s.GetOfficesFilterAsync(specialization, atWorkOnly), Times.Once);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("b8ef3a77-4c40-4b53-9a74-d4b68bbbc5d2", true)]
    public async Task GetSpecializationsFilter_ShouldCallService_AndReturnOkResult(string? officeIdString, bool atWorkOnly)
    {
        // Arrange
        Guid? officeId = officeIdString is null ? null : Guid.Parse(officeIdString);
        var expectedSpecs = new List<string> { "Cardiology", "Neurology" };

        _doctorServiceMock
            .Setup(s => s.GetSpecializationsFilterAsync(officeId, atWorkOnly))
            .ReturnsAsync(expectedSpecs);

        // Act
        var result = await _controller.GetSpecializationsFilter(officeId, atWorkOnly);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedSpecs);
        _doctorServiceMock.Verify(s => s.GetSpecializationsFilterAsync(officeId, atWorkOnly), Times.Once);
    }

    [Fact]
    public async Task GetDoctorById_ShouldCallServiceForPatient_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var officeId = Guid.NewGuid();
        var expectedDoctor = new DoctorDetailResponseDto(
            id,
            "photo.png",
            "John",
            "Doe",
            null,
            "John Doe",
            "Cardiology",
            10,
            officeId,
            "Room 101",
            new List<string> { "Consultation", "ECG" }
        );

        _doctorServiceMock
            .Setup(s => s.GetByIdForPatientAsync(id))
            .ReturnsAsync(expectedDoctor);

        // Act
        var result = await _controller.GetDoctorById(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedDoctor);
    }

    [Fact]
    public async Task GetDoctorByIdForAdmin_ShouldCallServiceForAdmin_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var officeId = Guid.NewGuid();
        var expectedDoctor = new DoctorAdminResponseDto(
            id,
            "photo.png",
            "Jane",
            "Doe",
            "Middle",
            "Jane Middle Doe",
            new DateOnly(1980, 2, 10),
            "jane@clinic.com",
            "Cardiology",
            officeId,
            "Room 101",
            2015,
            10,
            DoctorStatus.AtWork
        );

        _doctorServiceMock
            .Setup(s => s.GetByIdForAdminAsync(id))
            .ReturnsAsync(expectedDoctor);

        // Act
        var result = await _controller.GetDoctorByIdForAdmin(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedDoctor);
    }

    [Fact]
    public async Task GetDoctorByIdForOwner_ShouldCallServiceForOwner_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var officeId = Guid.NewGuid();
        var expectedDoctor = new DoctorOwnerResponseDto(
            id,
            "photo.png",
            "Jane",
            "Doe",
            "Middle",
            "Jane Middle Doe",
            new DateOnly(1980, 2, 10),
            "jane@clinic.com",
            "Cardiology",
            officeId,
            "Room 101",
            2015,
            10
        );

        _doctorServiceMock
            .Setup(s => s.GetByIdForOwnerAsync(id))
            .ReturnsAsync(expectedDoctor);

        // Act
        var result = await _controller.GetDoctorByIdForOwner(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedDoctor);
    }

    [Fact]
    public async Task CreateDoctor_ShouldCallService_AndReturnCreatedAtActionResult()
    {
        // Arrange
        var officeId = Guid.NewGuid();
        var dto = new CreateDoctorDto(
            "photo.png",
            "Jane",
            "Doe",
            null,
            new DateOnly(1985, 4, 15),
            "jane@clinic.com",
            "Cardiology",
            officeId,
            2020,
            DoctorStatus.AtWork
        );

        var createdId = Guid.NewGuid();
        var createdDoctor = new DoctorAdminResponseDto(
            createdId,
            dto.PhotoUrl,
            dto.FirstName,
            dto.LastName,
            dto.MiddleName,
            "Jane Doe",
            dto.DateOfBirth,
            dto.Email,
            dto.Specialization,
            dto.OfficeId,
            "Room 101",
            dto.CareerStartYear,
            5,
            dto.Status
        );

        _doctorServiceMock
            .Setup(s => s.CreateAsync(dto))
            .ReturnsAsync(createdDoctor);

        // Act
        var result = await _controller.CreateDoctor(dto);

        // Assert
        var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.ActionName.Should().Be(nameof(DoctorsController.GetDoctorByIdForAdmin));
        createdAtResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(createdId);
        createdAtResult.Value.Should().BeEquivalentTo(createdDoctor);
    }

    [Fact]
    public async Task UpdateDoctor_ShouldCallService_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var officeId = Guid.NewGuid();
        var dto = new UpdateDoctorDto(
            "new_photo.png",
            "Jane",
            "Updated",
            null,
            new DateOnly(1985, 4, 15),
            "jane@clinic.com",
            "Neurology",
            officeId,
            2020
        );

        var updatedDoctor = new DoctorAdminResponseDto(
            id,
            dto.PhotoUrl,
            dto.FirstName,
            dto.LastName,
            dto.MiddleName,
            "Jane Updated",
            dto.DateOfBirth,
            dto.Email,
            dto.Specialization,
            dto.OfficeId,
            "Room 202",
            dto.CareerStartYear,
            5,
            DoctorStatus.OnVacation
        );

        _doctorServiceMock
            .Setup(s => s.UpdateAsync(id, dto))
            .ReturnsAsync(updatedDoctor);

        // Act
        var result = await _controller.UpdateDoctor(id, dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(updatedDoctor);
    }

    [Fact]
    public async Task ChangeDoctorStatus_ShouldCallService_AndReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new ChangeDoctorStatusDto(DoctorStatus.Inactive);

        _doctorServiceMock
            .Setup(s => s.ChangeStatusAsync(id, dto.Status))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ChangeDoctorStatus(id, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _doctorServiceMock.Verify(s => s.ChangeStatusAsync(id, dto.Status), Times.Once);
    }
}
