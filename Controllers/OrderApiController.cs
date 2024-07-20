using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using BuyAndSell.Models;
using BuyAndSell.Data;
using BuyAndSell.Contracts;

namespace BuyAndSell.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetOrdersByOwnerId/{ownerId}")]
        public IActionResult GetOrdersByOwnerId(string ownerId)
        {
            var orders = _context.Orders.Where(o => o.OwnerId == ownerId).ToList();
            if (orders == null || orders.Count == 0)
            {
                return NotFound("Замовлення не знайдено");
            }

            return Ok(orders);
        }

        [HttpDelete("DeleteOrder/{orderId}")]
        public IActionResult DeleteOrder(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound("Замовлення не знайдено");
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return Ok("Замовлення успішно видалено");
        }

        [HttpPost]
        public IActionResult CreateOrder(OrderRequest orderRequest)
        {
            var ad = _context.Ads.FirstOrDefault(a => a.Id == orderRequest.AdId);
            if (ad == null)
            {
                return NotFound("Замовлення не знайдено");
            }

            var order = new Order
            {
                FullName = orderRequest.FullName,
                PhoneNumber = orderRequest.PhoneNumber,
                PaymentMethod = orderRequest.PaymentMethod,
                PostOffice = orderRequest.PostOffice,
                AdId = orderRequest.AdId,
                ProductName = ad.Title,
                ProductPrice = ad.Price,
                OwnerId = ad.UserId,
                CreatedAt = DateTime.UtcNow
            };
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Orders.Add(order);
            _context.SaveChanges();

            return Ok("Замовлення успішно створено");
        }
    }
}
