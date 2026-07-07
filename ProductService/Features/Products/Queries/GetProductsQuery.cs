using MediatR;
using ProductService.Models;

namespace ProductService.Features.Products.Queries;

public record GetProductsQuery : IRequest<List<Product>>;