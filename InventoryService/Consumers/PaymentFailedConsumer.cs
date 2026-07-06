using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;

namespace InventoryService.Consumers;

public class PaymentFailedConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
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
            queue: "payment-failed",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, e) =>
        {
            var json =
                Encoding.UTF8.GetString(
                    e.Body.ToArray());

            Console.WriteLine(
                $"❌ Payment Failed Event Received: {json}");

            var failedEvent =
                JsonSerializer.Deserialize<PaymentFailedEvent>(
                    json);

            if (failedEvent is null)
                return;

            Console.WriteLine(
                $"🔄 Releasing Inventory For Order {failedEvent.OrderId}");

    //        Console.WriteLine(
    //$"Publishing InventoryReleasedEvent: {releaseJson}");

            var releasedEvent =
                new InventoryReleasedEvent
                {
                    OrderId = failedEvent.OrderId,
                    ProductId = failedEvent.ProductId,
                    Quantity = failedEvent.Quantity
                };

            var releaseJson =
                JsonSerializer.Serialize(releasedEvent);

            Console.WriteLine(
            $"Publishing InventoryReleasedEvent: {releaseJson}");

            var releaseBody =
                Encoding.UTF8.GetBytes(releaseJson);

            await channel.QueueDeclareAsync(
                queue: "inventory-released",
                durable: true,
                exclusive: false,
                autoDelete: false);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "inventory-released",
                body: releaseBody);

            Console.WriteLine(
                $"✅ Inventory Released For Order {failedEvent.OrderId}");
        };

        await channel.BasicConsumeAsync(
            queue: "payment-failed",
            autoAck: true,
            consumer: consumer);

        Console.WriteLine(
            "Listening on queue: payment-failed");

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}