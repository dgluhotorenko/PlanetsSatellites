using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PlanetService.AsyncDataServices.Abstract;
using PlanetService.Controllers;
using PlanetService.Data.Abstract;
using PlanetService.DTOs;
using PlanetService.Models;
using PlanetService.SyncDataServices.Http.Abstract;

namespace PlanetService.Tests;

public class PlanetControllerTests
{
    private readonly Mock<IPlanetRepository> _repositoryMock = new();
    private readonly Mock<IHttpDataClient> _httpClientMock = new();
    private readonly Mock<IMessageBusDataClient> _messageBusMock = new();
    private readonly Mock<ILogger<PlanetController>> _loggerMock = new();
    private readonly PlanetController _controller;

    public PlanetControllerTests()
    {
        _controller = new PlanetController(
            _repositoryMock.Object,
            _httpClientMock.Object,
            _messageBusMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void GetAll_ReturnsOkWithPlanets()
    {
        var planets = new List<Planet>
        {
            new() { Id = 1, Name = "Earth", Mass = 1.0, Radius = 6371.0 }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(planets);

        var result = _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<PlanetReadDto>>(okResult.Value);
        Assert.Single(dtos);
    }

    [Fact]
    public void GetById_ExistingPlanet_ReturnsOk()
    {
        var planet = new Planet { Id = 1, Name = "Earth", Mass = 1.0, Radius = 6371.0 };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(planet);

        var result = _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<PlanetReadDto>(okResult.Value);
        Assert.Equal("Earth", dto.Name);
    }

    [Fact]
    public void GetById_NonExistingPlanet_ReturnsNotFound()
    {
        _repositoryMock.Setup(r => r.GetById(999)).Returns((Planet?)null);

        var result = _controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_ValidPlanet_ReturnsCreatedAtAction()
    {
        var createDto = new PlanetCreateDto { Name = "Mars", Mass = 0.107, Radius = 3389.5 };
        _repositoryMock.Setup(r => r.SaveChanges()).Returns(true);

        var result = await _controller.CreateAsync(createDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<PlanetReadDto>(createdResult.Value);
        Assert.Equal("Mars", dto.Name);
        _repositoryMock.Verify(r => r.Create(It.IsAny<Planet>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_HttpClientFails_StillReturnsCreated()
    {
        var createDto = new PlanetCreateDto { Name = "Mars", Mass = 0.107, Radius = 3389.5 };
        _httpClientMock
            .Setup(h => h.SendPlanetDataAsync(It.IsAny<PlanetReadDto>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        var result = await _controller.CreateAsync(createDto);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_MessageBusFails_StillReturnsCreated()
    {
        var createDto = new PlanetCreateDto { Name = "Mars", Mass = 0.107, Radius = 3389.5 };
        _messageBusMock
            .Setup(m => m.PublishNewPlanetAsync(It.IsAny<PlanetPublishedDto>()))
            .ThrowsAsync(new Exception("RabbitMQ unavailable"));

        var result = await _controller.CreateAsync(createDto);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }
}
