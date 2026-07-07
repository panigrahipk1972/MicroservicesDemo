using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Features.Products.Queries;

public class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, List<Product>>
{
    private readonly ProductDbContext _context;

    public GetProductsQueryHandler(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }
}