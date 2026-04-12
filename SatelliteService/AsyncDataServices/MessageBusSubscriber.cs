using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SatelliteService.EventProcessing.Abstract;

namespace SatelliteService.AsyncDataServices;

public sealed class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<MessageBusSubscriber> _logger;
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;

    public MessageBusSubscriber(
        IConfiguration configuration,
        IEventProcessor eventProcessor,
        ILogger<MessageBusSubscriber> logger)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Retry connection — RabbitMQ may not be ready yet at startup
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await InitializeRabbitMqAsync();
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RabbitMQ not available — retrying in 5 seconds");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        if (_channel is null || _queueName is null)
            return;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            _logger.LogInformation("Event received from message bus");

            try
            {
                _eventProcessor.ProcessEvent(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message bus event");
            }
        };

        await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer, stoppingToken);

        // Keep the service alive until cancellation is requested
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task InitializeRabbitMqAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQHost"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQPort"] ?? "5672")
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(MessageBusConstants.ExchangeName, ExchangeType.Fanout);
        _queueName = (await _channel.QueueDeclareAsync()).QueueName;
        await _channel.QueueBindAsync(_queueName, MessageBusConstants.ExchangeName, string.Empty);

        _connection.ConnectionShutdownAsync += (_, args) =>
        {
            _logger.LogWarning("RabbitMQ connection shut down: {Reason}", args.ReplyText);
            return Task.CompletedTask;
        };

        _logger.LogInformation("Connected to RabbitMQ at {Host}:{Port}",
            factory.HostName, factory.Port);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
            await _channel.CloseAsync(cancellationToken);

        if (_connection is { IsOpen: true })
            await _connection.CloseAsync(cancellationToken);

        _channel?.Dispose();
        _connection?.Dispose();

        await base.StopAsync(cancellationToken);
    }
}
