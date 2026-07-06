using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Features.Orders.Commands;

namespace OrderService.Features.Orders.Handlers
{
    public class UpdateOrderCommandHandler :
        IRequestHandler<UpdateOrderCommand, bool>
    {
        private readonly OrderDbContext _context;

        public UpdateOrderCommandHandler(
            OrderDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            UpdateOrderCommand request,
            CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (order == null)
                return false;

            order.ProductId = request.ProductId;
            order.Quantity = request.Quantity;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}