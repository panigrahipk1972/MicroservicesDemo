using MediatR;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Features.Products.Commands;

public class CreateProductCommandHandler(ProductDbContext db)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };

        db.Products.Add(product);

        await db.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}