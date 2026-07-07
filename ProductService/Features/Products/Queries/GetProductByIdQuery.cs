using MediatR;
using ProductService.Models;

namespace ProductService.Features.Products.Queries;

public record GetProductByIdQuery(int Id)
    : IRequest<Product?>;
