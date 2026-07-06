using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;
using System.Text;
using System.Text.Json;

namespace InventoryService.Consumers;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(
        ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        var connection =
            await factory.CreateConnectionAsync(stoppingToken);

        var channel =
            await connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: "order-created",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: "inventory-reserved",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                Console.WriteLine("================================");
                Console.WriteLine("MESSAGE RECEIVED FROM order-created");
                Console.WriteLine("================================");

                var body =
                    args.Body.ToArray();

                var json =
                    Encoding.UTF8.GetString(body);

                Console.WriteLine("OrderCreatedEvent Payload:");
                Console.WriteLine(json);

                var order =
                    JsonSerializer.Deserialize<OrderCreatedEvent>(json);

                if (order is null)
                {
                    Console.WriteLine("OrderCreatedEvent is null");
                    return;
                }

                Console.WriteLine($"OrderId   = {order.OrderId}");
                Console.WriteLine($"ProductId = {order.ProductId}");
                Console.WriteLine($"Quantity  = {order.Quantity}");

                // Here we are assuming inventory is reserved successfully.
                // Later we will add actual DB stock check.
                var inventoryReservedEvent =
                    new InventoryReservedEvent
                    {
                        OrderId = order.OrderId,
                        ProductId = order.ProductId,
                        Quantity = order.Quantity
                    };

                var eventJson =
                    JsonSerializer.Serialize(inventoryReservedEvent);

                var eventBody =
                    Encoding.UTF8.GetBytes(eventJson);

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "inventory-reserved",
                    body: eventBody,
                    cancellationToken: stoppingToken);

                Console.WriteLine("================================");
                Console.WriteLine("InventoryReservedEvent Published");
                Console.WriteLine(eventJson);
                Console.WriteLine("================================");

                _logger.LogInformation(
                    "InventoryReservedEvent Published For Order {OrderId}",
                    order.OrderId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR IN OrderCreatedConsumer");
                Console.WriteLine(ex.ToString());

                _logger.LogError(
                    ex,
                    "Error while consuming order-created message");
            }

            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(
            queue: "order-created",
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);

        Console.WriteLine("RabbitMQ Consumer Started...");
        Console.WriteLine("Listening on queue: order-created");

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}