using MediatR;

namespace ProductService.Features.Products.Commands;

public record DeleteProductCommand(int Id)
    : IRequest<bool>;