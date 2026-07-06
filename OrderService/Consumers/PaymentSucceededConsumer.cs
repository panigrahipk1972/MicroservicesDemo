using OrderService.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace OrderService.Consumers
{
    public class PaymentSucceededConsumer
     : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentSucceededConsumer(
            IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            var connection =
                factory.CreateConnection();

            var channel =
                connection.CreateModel();

            channel.QueueDeclare(
                queue: "payment-success",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer =
                new EventingBasicConsumer(channel);

            consumer.Received += async (sender, e) =>
            {
                try
                {
                    //Console.WriteLine("================================");
                    //Console.WriteLine("INVENTORY RELEASED EVENT RECEIVED");
                    //Console.WriteLine("================================");

                    var json =
                        Encoding.UTF8.GetString(
                            e.Body.ToArray());

                    Console.WriteLine(json);

                    var releasedEvent =
                        JsonSerializer.Deserialize<PaymentSucceededEvent>(
                            json);

                    if (releasedEvent is null)
                    {
                        Console.WriteLine(
                            "InventoryReleasedEvent is null");

                        return;
                    }

                    Console.WriteLine(
                        $"Searching OrderId {releasedEvent.OrderId}");

                    using var scope =
                        _scopeFactory.CreateScope();

                    var db =
                        scope.ServiceProvider
                            .GetRequiredService<OrderDbContext>();

                    var order =
                        await db.Orders
                            .FirstOrDefaultAsync(
                                x => x.Id ==
                                     releasedEvent.OrderId);

                    if (order is null)
                    {
                        Console.WriteLine(
                            $"Order {releasedEvent.OrderId} NOT FOUND");

                        return;
                    }

                    Console.WriteLine(
                        $"Current Status: {order.Status}");

                    order.Status = "Completed";

                    await db.SaveChangesAsync();

                    Console.WriteLine(
                        $"🚫 Order {order.Id} Completed");

                    Console.WriteLine(
                        $"New Status: {order.Status}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        "ERROR IN InventoryReleasedConsumer");

                    Console.WriteLine(ex);
                }
            };

            Console.WriteLine(
     "Registering payment-success consumer...");

            channel.BasicConsume(
                queue: "payment-success",
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(
    "Payment-success consumer registered.");

            Console.WriteLine(
      "Listening on queue: payment-success");

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
    }
}
