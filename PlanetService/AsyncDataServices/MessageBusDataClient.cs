using System.Text;
using System.Text.Json;
using PlanetService.AsyncDataServices.Abstract;
using PlanetService.DTOs;
using RabbitMQ.Client;

namespace PlanetService.AsyncDataServices;

public sealed class MessageBusDataClient : IMessageBusDataClient
{
    private readonly ILogger<MessageBusDataClient> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _disposed;

    // SemaphoreSlim ensures thread-safe lazy initialization of the RabbitMQ connection
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public MessageBusDataClient(IConfiguration configuration, ILogger<MessageBusDataClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishNewPlanetAsync(PlanetPublishedDto planetPublishedDto)
    {
        await EnsureConnectedAsync();

        if (_channel is not { IsOpen: true })
        {
            _logger.LogWarning("RabbitMQ channel is not available — message not sent");
            return;
        }

        var message = JsonSerializer.Serialize(planetPublishedDto);
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            MessageBusConstants.ExchangeName, routingKey: string.Empty, mandatory: false, body: body);

        _logger.LogInformation("Published message to RabbitMQ: {Message}", message);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_channel is { IsOpen: true })
            await _channel.CloseAsync();

        if (_connection is { IsOpen: true })
            await _connection.CloseAsync();

        _channel?.Dispose();
        _connection?.Dispose();
        _initLock.Dispose();

        _logger.LogInformation("RabbitMQ connection disposed");
    }

    private async Task EnsureConnectedAsync()
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
            return;

        await _initLock.WaitAsync();
        try
        {
            if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
                return;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQPort"] ?? "5672")
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(MessageBusConstants.ExchangeName, ExchangeType.Fanout);

            _connection.ConnectionShutdownAsync += (_, args) =>
            {
                _logger.LogWarning("RabbitMQ connection shut down: {Reason}", args.ReplyText);
                return Task.CompletedTask;
            };

            _logger.LogInformation("Connected to RabbitMQ at {Host}:{Port}",
                factory.HostName, factory.Port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not connect to RabbitMQ");
        }
        finally
        {
            _initLock.Release();
        }
    }
}
