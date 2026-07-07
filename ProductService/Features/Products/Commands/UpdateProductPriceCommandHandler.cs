using MediatR;
using ProductService.Data;

namespace ProductService.Features.Products.Commands
{
    public class UpdateProductPriceCommandHandler(ProductDbContext db)
     : IRequestHandler<UpdateProductPriceCommand, bool>
    {
        public async Task<bool> Handle(
            UpdateProductPriceCommand request,
            CancellationToken cancellationToken)
        {
            var product = await db.Products.FindAsync(request.Id);

            if (product is null)
                return false;

            product.Price = request.Price;

            await db.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
