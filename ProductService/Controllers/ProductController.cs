using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Data;
using ProductService.Features.Products.Commands;
using ProductService.Features.Products.Queries;
using ProductService.Models;

namespace ProductService.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class ProductController : ControllerBase
//{
//    private readonly ProductDbContext _context;
//    private readonly IMediator _mediator;

//    public ProductController(ProductDbContext context, IMediator mediator)
//    {
//        _mediator = mediator;
//        _context = context;
//    }

//    [HttpGet]
//    public IActionResult GetProducts()
//    {
//        return Ok(_context.Products.ToList());
//    }

//    //[HttpPost]
//    //public async Task<IActionResult> CreateProduct(Product product)
//    //{
//    //    _context.Products.Add(product);

//    //    await _context.SaveChangesAsync();

//    //    return Ok(product);
//    //}
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController
     : ControllerBase
    {

    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
        public async Task<IActionResult> CreateProduct(
            CreateProductCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

    //[HttpPut("{id}")]
    //public async Task<IActionResult> UpdateProduct(
    //    int id,
    //    UpdateProductCommand command)
    //{
    //    if (id != command.Id)
    //        return BadRequest();

    //    var result = await _mediator.Send(command);

    //    return result
    //        ? NoContent()
    //        : NotFound();
    //}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(
int id,
[FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        var result = await _mediator.Send(command);

        return result
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _mediator.Send(
                new DeleteProductCommand(id));

            return result
                ? NoContent()
                : NotFound();
        }
        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(
    int id,
    UpdateProductPriceCommand command)
        {
            var result = await _mediator.Send(command);

            return result
                ? NoContent()
                : NotFound();
        }
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products =
            await _mediator.Send(
                new GetProductsQuery());

        return Ok(products);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product =
            await _mediator.Send(
                new GetProductByIdQuery(id));

        if (product == null)
            return NotFound();

        return Ok(product);
    }
}