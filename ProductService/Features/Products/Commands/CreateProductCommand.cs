using MediatR;

namespace ProductService.Features.Products.Commands;

public record CreateProductCommand(
    string Name,
    decimal Price
) : IRequest<int>;