using MediatR;
namespace ProductService.Features.Products.Commands;
public record UpdateProductPriceCommand(
    int Id,
    decimal Price
) : IRequest<bool>;