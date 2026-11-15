using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Profiles.Api.Controllers;
using Profiles.Application.DTOs.ReceptionistDtos;
using Profiles.Application.Services;

namespace Profiles.Api.Tests.Controllers;

public class ReceptionistsControllerTests
{
    private readonly Mock<IReceptionistService> _receptionistServiceMock;
    private readonly ReceptionistsController _controller;

    public ReceptionistsControllerTests()
    {
        _receptionistServiceMock = new Mock<IReceptionistService>();
        _controller = new ReceptionistsController(_receptionistServiceMock.Object);
    }

    [Fact]
    public async Task GetReceptionists_ShouldCallService_AndReturnOkResult()
    {
        // Arrange
        var expectedList = new List<ReceptionistListItemDto>
        {
            new(Guid.NewGuid(), "Jane Receptionist", Guid.NewGuid(), "123 Main St")
        };

        _receptionistServiceMock
            .Setup(s => s.GetListAsync())
            .ReturnsAsync(expectedList);

        // Act
        var result = await _controller.GetReceptionists();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public async Task GetReceptionistById_ShouldCallService_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new ReceptionistResponseDto(
            id,
            "photo.jpg",
            "Jane",
            "Doe",
            null,
            "Jane Doe",
            "jane@clinic.com",
            Guid.NewGuid(),
            "123 Main St"
        );

        _receptionistServiceMock
            .Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetReceptionistById(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateReceptionist_ShouldCallService_AndReturnCreatedAtActionResult()
    {
        // Arrange
        var officeId = Guid.NewGuid();
        var dto = new CreateReceptionistDto("photo.jpg", "Jane", "Doe", null, "jane@clinic.com", officeId);
        var created = new ReceptionistResponseDto(
            Guid.NewGuid(),
            dto.PhotoUrl,
            dto.FirstName,
            dto.LastName,
            dto.MiddleName,
            "Jane Doe",
            dto.Email,
            officeId,
            "123 Main St"
        );

        _receptionistServiceMock
            .Setup(s => s.CreateAsync(dto))
            .ReturnsAsync(created);

        // Act
        var result = await _controller.CreateReceptionist(dto);

        // Assert
        var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.ActionName.Should().Be(nameof(ReceptionistsController.GetReceptionistById));
        createdAtResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(created.Id);
        createdAtResult.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task UpdateReceptionist_ShouldCallService_AndReturnOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var officeId = Guid.NewGuid();
        var dto = new UpdateReceptionistDto("new_photo.jpg", "Jane", "Updated", null, officeId);
        var updated = new ReceptionistResponseDto(
            id,
            dto.PhotoUrl,
            dto.FirstName,
            dto.LastName,
            dto.MiddleName,
            "Jane Updated",
            "jane@clinic.com",
            officeId,
            "456 Other St"
        );

        _receptionistServiceMock
            .Setup(s => s.UpdateAsync(id, dto))
            .ReturnsAsync(updated);

        // Act
        var result = await _controller.UpdateReceptionist(id, dto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(updated);
    }

    [Fact]
    public async Task DeleteReceptionist_ShouldCallService_AndReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();

        _receptionistServiceMock
            .Setup(s => s.DeleteAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteReceptionist(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _receptionistServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
    }
}
