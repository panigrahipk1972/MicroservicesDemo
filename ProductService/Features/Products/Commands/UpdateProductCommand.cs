using MediatR;

namespace ProductService.Features.Products.Commands;

public class UpdateProductCommand : IRequest<bool>
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}