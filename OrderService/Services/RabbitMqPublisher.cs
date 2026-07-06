using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
namespace OrderService.Services;
public class RabbitMqPublisher
{
    public void Publish<T>(string queueName, T message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        using var connection =
            factory.CreateConnection();

        using var channel =
            connection.CreateModel();

        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json =
            JsonSerializer.Serialize(message);

        

        var body =
            Encoding.UTF8.GetBytes(json);
        Console.WriteLine($"Publishing to queue: {queueName}");
        Console.WriteLine($"Payload: {json}");
        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: null,
            body: body);

        Console.WriteLine($"Published: {json}");
    }
}