using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;

namespace ProductService.Features.Products.Commands;

public class UpdateProductCommandHandler(ProductDbContext db)
    : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.Id,
                cancellationToken);

        if (product is null)
            return false;

        product.Name = request.Name;
        product.Price = request.Price;

        await db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
