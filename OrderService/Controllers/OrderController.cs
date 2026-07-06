using MediatR;
using Microsoft.AspNetCore.Mvc;
//using OrderService.Data;
using OrderService.Features.Orders.Commands;
//using OrderService.Models;
using OrderService.Features.Orders.Queries;
using Microsoft.AspNetCore.Authorization;

namespace OrderService.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    //private readonly OrderDbContext _context;
    private readonly IHttpClientFactory _factory;
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator, IHttpClientFactory factory)
    {
       // _context = context;
        _factory = factory;
        _mediator=mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var orders = await _mediator.Send(
            new GetOrdersQuery());

        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
    CreateOrderCommand command)
    {
        var orderId =
            await _mediator.Send(command);

        return Ok(new
        {
            OrderId = orderId
        });
    }
    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var client = _factory.CreateClient("ProductService");

        var response =
            await client.GetAsync("/api/Product");

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadAsStringAsync();

        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order =
            await _mediator.Send(new GetOrderByIdQuery(id));

        if (order == null)
            return NotFound();

        return Ok(order);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,UpdateOrderCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        var result =
            await _mediator.Send(command);

        if (!result)
            return NotFound();

        return NoContent();
    }
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> Delete(int id)
    //{
    //    var result =
    //        await _mediator.Send(
    //            new DeleteOrderCommand(id));

    //    if (!result)
    //        return NotFound();

    //    return NoContent();
    //}
    //[HttpDelete]
    //public IActionResult Delete()
    //{
    //    return Ok("Delete Hit");
    //}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result =
            await _mediator.Send(
                new DeleteOrderCommand(id));

        if (!result)
            return NotFound();

        return NoContent();
    }
}