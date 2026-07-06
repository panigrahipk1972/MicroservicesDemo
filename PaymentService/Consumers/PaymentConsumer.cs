using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;

namespace PaymentService.Consumers;

public class PaymentConsumer : BackgroundService
{
    private readonly ILogger<PaymentConsumer> _logger;

    public PaymentConsumer(
        ILogger<PaymentConsumer> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            var connection =
                await factory.CreateConnectionAsync();

            var channel =
                await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "inventory-reserved",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var consumer =
                new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, e) =>
            {
                try
                {
                    Console.WriteLine("================================");
                    Console.WriteLine("PAYMENT MESSAGE RECEIVED");
                    Console.WriteLine("================================");

                    var body = e.Body.ToArray();

                    var message =
                        Encoding.UTF8.GetString(body);

                    Console.WriteLine(message);

                    var inventoryEvent =
                        JsonSerializer.Deserialize<InventoryReservedEvent>(
                            message);

                    if (inventoryEvent is null)
                    {
                        Console.WriteLine(
                            "InventoryReservedEvent is null");

                        return;
                    }

                    Console.WriteLine(
                        $"OrderId   = {inventoryEvent.OrderId}");

                    Console.WriteLine(
                        $"ProductId = {inventoryEvent.ProductId}");

                    Console.WriteLine(
                        $"Quantity  = {inventoryEvent.Quantity}");

                    _logger.LogInformation(
                        "Inventory Reserved received for Order: {OrderId}",
                        inventoryEvent.OrderId);

                    await ProcessPayment(
                        inventoryEvent,
                        channel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        "ERROR IN PAYMENT CONSUMER");

                    Console.WriteLine(ex);

                    _logger.LogError(
                        ex,
                        "Error while processing payment");
                }
            };

            await channel.BasicConsumeAsync(
                queue: "inventory-reserved",
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(
                "Payment Consumer Started...");

            Console.WriteLine(
                "Listening on queue: inventory-reserved");

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "ERROR STARTING PAYMENT CONSUMER");

            Console.WriteLine(ex);

            _logger.LogError(
                ex,
                "Error while starting Payment Consumer");
        }
    }

    private async Task ProcessPayment(
        InventoryReservedEvent evt,
        IChannel channel)
    {
        var random = new Random();

        //bool paymentSuccess =
        //    random.Next(1, 10) > 3;
        //bool paymentSuccess = false;
        bool paymentSuccess = true;

        if (paymentSuccess)
        {
            await PublishPaymentSuccess(
                evt,
                channel);
        }
        else
        {
            await PublishPaymentFailed(
                evt,
                channel);
        }
    }

    private async Task PublishPaymentSuccess(
        InventoryReservedEvent evt,
        IChannel channel)
    {
        var successEvent =
            new PaymentSucceededEvent
            {
                OrderId = evt.OrderId
            };

        var json =
            JsonSerializer.Serialize(successEvent);

        var body =
            Encoding.UTF8.GetBytes(json);

        await channel.QueueDeclareAsync(
            queue: "payment-success",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "payment-success",
            body: body);

        Console.WriteLine(
            $"✅ Payment Success For Order {evt.OrderId}");

        _logger.LogInformation(
            "Payment Success For Order {OrderId}",
            evt.OrderId);
    }

    private async Task PublishPaymentFailed(
        InventoryReservedEvent evt,
        IChannel channel)
    {
        var failedEvent =
            new PaymentFailedEvent
            {
                OrderId = evt.OrderId,
                ProductId = evt.ProductId,
                Quantity = evt.Quantity,
                Reason = "Insufficient Funds"
            };

        var json =
            JsonSerializer.Serialize(failedEvent);

        var body =
            Encoding.UTF8.GetBytes(json);

        await channel.QueueDeclareAsync(
            queue: "payment-failed",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "payment-failed",
            body: body);

        Console.WriteLine(
            $"❌ Payment Failed For Order {evt.OrderId}");

        _logger.LogInformation(
            "Payment Failed For Order {OrderId}",
            evt.OrderId);
    }
}