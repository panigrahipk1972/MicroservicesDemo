using MediatR;

namespace OrderService.Features.Orders.Commands
{
    public class UpdateOrderCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}