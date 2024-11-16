using System.Text;
using System.Text.Json;
using PlanetService.AsyncDataServices.Abstract;
using PlanetService.DTOs;
using RabbitMQ.Client;

namespace PlanetService.AsyncDataServices;

public class MessageBusDataClient(IConfiguration configuration) : IMessageBusDataClient
{
    private IConnection? _connection;
    private IChannel? _channel;
    private const string ExchangeName = "trigger";

    public async Task InitializeAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQHost"]!,
            Port = int.Parse(configuration["RabbitMQPort"]!)
        };

        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout);

            _connection.ConnectionShutdownAsync += (_, _) =>
            {
                Console.WriteLine("==> RabbitMQ Connection Shutdown");
                return Task.CompletedTask;
            };

            Console.WriteLine("==> Connected to MessageBus");

        }
        catch (Exception e)
        {
            Console.WriteLine($"==> Could not connect to MessageBus: {e.Message}");
        }
    }

    public async Task PublishNewPlanetAsync(PlanetPublishedDto planetPublishedDto)
    {
        var message = JsonSerializer.Serialize(planetPublishedDto);

        if (_connection is { IsOpen: true })
        {
            Console.WriteLine("==> RabbitMQ Connection is open, sending message...");
            await SendMessageAsync(message);
        }
        else
        {
            Console.WriteLine("==> RabbitMQ Connection is closed, cannot send message.");
        }
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
    }

    private async Task SendMessageAsync(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        if (_channel != null)
        {
            await _channel.BasicPublishAsync(ExchangeName, string.Empty, false, body);
            Console.WriteLine($"==> We have sent {message}");
        }
    }
}