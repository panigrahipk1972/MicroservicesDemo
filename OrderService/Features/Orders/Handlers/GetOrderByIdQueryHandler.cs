using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Features.Orders.Queries;
using OrderService.Models;

namespace OrderService.Features.Orders.Handlers
{
    public class GetOrderByIdQueryHandler :
        IRequestHandler<GetOrderByIdQuery, Order?>
    {
        private readonly OrderDbContext _context;

        public GetOrderByIdQueryHandler(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == request.Id);
        }
    }
}