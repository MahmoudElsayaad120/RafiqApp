using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.OrderModels;

namespace Presentation
{


    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) 
        {
            _orderService = orderService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderRequestDto request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _orderService.CreateOrderAsync(request, email);
            return Ok(request);

        }

        [HttpGet]
        public async Task<IActionResult> GetOrders() 
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _orderService.GetOrdersByUserEmailAsync(email);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id) 
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            return Ok(result);
        }
    }
}
