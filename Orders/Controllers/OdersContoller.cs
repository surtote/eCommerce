using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.DTOs;
using Orders.Models;
using Orders.Services;
using static Orders.DTOs.OrdersDTO;
using System.Security.Claims;

namespace Orders.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private string? GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            userId = User.FindFirst("sub")?.Value;
        }
        if (string.IsNullOrEmpty(userId))
        {
            userId = User.FindFirst("user_id")?.Value;
        }

        // DEBUG: Log para verificar qué claims llegan
        var allClaims = User.Claims.Select(c => $"{c.Type}={c.Value}");
        System.Diagnostics.Debug.WriteLine($"All Claims: {string.Join(", ", allClaims)}");

        return userId;
    }

    [HttpGet("my")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new { message = "User ID not found in token" });

        var result = await _orderService.GetUserOrdersAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new { message = "User ID not found in token" });

        var result = await _orderService.GetByIdAsync(id, userId);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new { message = "User ID not found in token" });

        var result = await _orderService.CreateAsync(userId, request);
        return result.Succeeded ? Created(nameof(GetById), result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/cancel")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new { message = "User ID not found in token" });

        var result = await _orderService.CancelAsync(id, userId);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAll([FromQuery] OrderStatus? status, [FromQuery] string? userId)
    {
        var result = await _orderService.GetAllAsync(status, userId);
        return Ok(result);
    }

    [HttpGet("admin/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetByIdForAdmin(Guid id)
    {
        var result = await _orderService.GetByIdForAdminAsync(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpPut("admin/{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var result = await _orderService.UpdateStatusAsync(id, request.Status);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}