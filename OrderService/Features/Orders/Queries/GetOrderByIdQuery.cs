using MediatR;
using OrderService.Models;

namespace OrderService.Features.Orders.Queries
{
    public class GetOrderByIdQuery : IRequest<Order?>
    {
        public int Id { get; set; }

        public GetOrderByIdQuery(int id)
        {
            Id = id;
        }
    }
}