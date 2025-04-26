using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTO;
using OrderService.Application;
using OrderService.Domain;

namespace OrderService.API;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder(OrderToCreate orderToCreate)
    {
        var order = mapper.Map<Order>(orderToCreate);
        var createdOrder = await orderService.CreateOrderAsync(order).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllOrders() =>
        Ok(await orderService.GetAllOrdersAsync().ConfigureAwait(false));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrder([Required]Guid id)
    {
        var order = await orderService.GetOrderByIdAsync(id).ConfigureAwait(false);
        return order is not null ? Ok(order) : NotFound();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateOrder(OrderToUpdate orderToUpdate)
    {
        var order = mapper.Map<Order>(orderToUpdate);
        var orderFound = await orderService.GetOrderByIdAsync(order.Id).ConfigureAwait(false);
        if (orderFound is null) return NotFound();
        var orderModified = await orderService.UpdateOrderAsync(order).ConfigureAwait(false);
        return Ok(orderModified);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteOrder([Required]Guid id)
    {
        var order = await orderService.GetOrderByIdAsync(id).ConfigureAwait(false);
        if (order is null) return  NotFound();
        await orderService.DeleteOrderAsync(id).ConfigureAwait(false);
        return NoContent();
    }
}