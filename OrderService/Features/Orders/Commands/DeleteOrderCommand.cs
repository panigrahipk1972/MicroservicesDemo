using MediatR;

namespace OrderService.Features.Orders.Commands
{
    public class DeleteOrderCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteOrderCommand(int id)
        {
            Id = id;
        }
    }
}