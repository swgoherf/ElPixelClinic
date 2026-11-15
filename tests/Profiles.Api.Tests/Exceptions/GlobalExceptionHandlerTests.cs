using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Profiles.Api.Exceptions;
using Profiles.Application.Common.Exceptions;

namespace Profiles.Api.Tests.Exceptions;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        _handler = new GlobalExceptionHandler(_loggerMock.Object);
    }

    [Theory]
    [MemberData(nameof(GetExceptionTestCases))]
    public async Task TryHandleAsync_ShouldSetCorrectStatusCodeAndProblemDetails(
        Exception exception,
        int expectedStatusCode,
        string expectedTitle,
        string expectedDetail)
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;
        context.Request.Path = "/api/test/resource";
        context.TraceIdentifier = "trace-12345";

        // Act
        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            responseBodyStream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(expectedStatusCode);
        problemDetails.Title.Should().Be(expectedTitle);
        problemDetails.Detail.Should().Be(expectedDetail);
        problemDetails.Instance.Should().Be("/api/test/resource");
        problemDetails.Extensions.Should().ContainKey("traceId");
        problemDetails.Extensions["traceId"]!.ToString().Should().Be("trace-12345");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    public static IEnumerable<object[]> GetExceptionTestCases()
    {
        yield return new object[]
        {
            new NotFoundException("Entity not found"),
            StatusCodes.Status404NotFound,
            "Not Found",
            "Entity not found"
        };
        yield return new object[]
        {
            new ValidationException("Validation failed for entity"),
            StatusCodes.Status400BadRequest,
            "Bad Request",
            "Validation failed for entity"
        };
        yield return new object[]
        {
            new UnauthorizedException("Access is unauthorized"),
            StatusCodes.Status401Unauthorized,
            "Unauthorized",
            "Access is unauthorized"
        };
        yield return new object[]
        {
            new ForbiddenException("You are not allowed here"),
            StatusCodes.Status403Forbidden,
            "Forbidden",
            "You are not allowed here"
        };
        yield return new object[]
        {
            new ConflictException("Conflict detected"),
            StatusCodes.Status409Conflict,
            "Conflict",
            "Conflict detected"
        };
        yield return new object[]
        {
            new InvalidOperationException("Something unexpected broke"),
            StatusCodes.Status500InternalServerError,
            "Internal Server Error",
            "An unexpected error occurred. Please try again later."
        };
    }
}
