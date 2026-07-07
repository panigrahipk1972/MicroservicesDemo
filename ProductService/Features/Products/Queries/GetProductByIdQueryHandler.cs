using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Features.Products.Queries;

public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly ProductDbContext _context;

    public GetProductByIdQueryHandler(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.Id,
                cancellationToken);
    }
}