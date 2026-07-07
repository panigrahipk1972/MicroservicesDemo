using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;

namespace ProductService.Features.Products.Commands;

public class DeleteProductCommandHandler(ProductDbContext db)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.Id,
                cancellationToken);

        if (product is null)
            return false;

        db.Products.Remove(product);

        await db.SaveChangesAsync(cancellationToken);

        return true;
    }
}