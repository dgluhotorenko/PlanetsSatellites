using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SatelliteService.EventProcessing.Abstract;

namespace SatelliteService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string ExchangeName = "trigger";
    private string? _queueName;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        InitializeMessageBusAsync().GetAwaiter().GetResult();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            Console.WriteLine("==> Event received from MessageBus");

            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            _eventProcessor.ProcessEvent(message);

            await Task.CompletedTask;
        };

        await _channel!.BasicConsumeAsync(_queueName!, true, consumer, stoppingToken);
    }

    private async Task InitializeMessageBusAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQHost"]!,
            Port = int.Parse(_configuration["RabbitMQPort"]!)
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout);
        _queueName = (await _channel.QueueDeclareAsync()).QueueName;
        await _channel.QueueBindAsync(_queueName, ExchangeName, string.Empty);

        Console.WriteLine("==> Connected to MessageBus");

        _connection.ConnectionShutdownAsync += (_, _) =>
        {
            Console.WriteLine("==> RabbitMQ Connection Shutdown");
            return Task.CompletedTask;
        };
    }

    public async Task DisposeAsync()
    {
        Console.WriteLine("==> Disposing MessageBus");

        if (_channel is { IsOpen: true })
        {
            await _channel.CloseAsync();
        }

        if (_connection is { IsOpen: true })
        {
            await _connection.CloseAsync();
        }

        base.Dispose();
    }
}