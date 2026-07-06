using MediatR;
using OrderService.Data;
using OrderService.Events;
using OrderService.Features.Orders.Commands;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Features.Orders.Handlers;

public class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, int>
{
    private readonly OrderDbContext _context;
    private readonly RabbitMqPublisher _publisher;
    public CreateOrderCommandHandler(
        OrderDbContext context, RabbitMqPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<int> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = new Order
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            OrderDate = DateTime.UtcNow,
            Status = "Created"
        };

        _context.Orders.Add(order);

        await _context.SaveChangesAsync(
    cancellationToken);

        var orderCreatedEvent =
     new OrderCreatedEvent
     {
         OrderId = order.Id,
         ProductId = order.ProductId,
         Quantity = order.Quantity
     };

        _publisher.Publish(
            "order-created",
            orderCreatedEvent);

        return order.Id;

        
    }
}