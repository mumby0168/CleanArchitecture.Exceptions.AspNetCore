using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CleanArchitecture.Exceptions.AspNetCore.Tests.Stubs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq.AutoMock;
using Newtonsoft.Json;
using Xunit;

namespace CleanArchitecture.Exceptions.AspNetCore.Tests;

public class CleanArchitectureExceptionsMiddlewareTests
{
    private AutoMocker _mocker = new();

    private CleanArchitectureExceptionsOptions _options = new()
    {
        ApplicationName = "TestApp"
    };

    public CleanArchitectureExceptionsMiddlewareTests()
    {
        _mocker.GetMock<IOptionsMonitor<CleanArchitectureExceptionsOptions>>()
            .SetupGet(o => o.CurrentValue).Returns(_options);
    }

    private IMiddleware CreateSut() => _mocker.CreateInstance<CleanArchitectureExceptionsMiddleware>();

    [Fact]
    public async Task InvokeAsync_ResourceNotFoundException_MapsTo404ResponseCorrectly()
    {
        //Arrange
        var sut = CreateSut();

        var context = new DefaultHttpContext();

        var exception = new ResourceNotFoundException<TestResource>("The resource was not found", "code_test");

        //Act
        await sut.InvokeAsync(context, _ => throw exception);

        //Assert
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task InvokeAsync_ResourceExistsException_MapsTo409ResponseCorrectly()
    {
        //Arrange
        var sut = CreateSut();

        var context = new DefaultHttpContext();

        var exception = new ResourceExistsException<TestResource>("The resource was not found", "code_test");

        //Act
        await sut.InvokeAsync(context, _ => throw exception);

        //Assert
        context.Response.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task InvokeAsync_DomainException_MapsTo400ResponseCorrectly()
    {
        //Arrange
        var sut = CreateSut();

        var context = new DefaultHttpContext();

        var exception = new DomainException<TestResource>("The resource was not found", "code_test");

        //Act
        await sut.InvokeAsync(context, _ => throw exception);

        //Assert
        context.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task InvokeAsync_CustomException_MapsTo403ResponseCorrectly()
    {
        //Arrange
        var sut = CreateSut();

        var context = new DefaultHttpContext();

        var exception = new CustomException("The resource was not found");

        _options.ConfigureCustomException<CustomException>(HttpStatusCode.Forbidden);

        //Act
        await sut.InvokeAsync(context, _ => throw exception);

        //Assert
        context.Response.StatusCode.Should().Be(403);
    }
}

public class CustomException : BaseCleanArchitectureException
{
    public CustomException(string message) : base(message)
    {
    }
}