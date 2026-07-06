using MediatR;
namespace OrderService.Features.Orders.Commands
{
    public class CreateOrderCommand: IRequest<int>
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
