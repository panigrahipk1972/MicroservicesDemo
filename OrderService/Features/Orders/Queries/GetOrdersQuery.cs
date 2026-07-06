using MediatR;
using OrderService.Models;

namespace OrderService.Features.Orders.Queries
{
    public class GetOrdersQuery : IRequest<List<Order>>
    {
    }
}