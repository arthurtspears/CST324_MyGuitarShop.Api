using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Repository;

namespace CST324_MyGuitarShop.Api.Controllers.AdoControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(
        ILogger<OrdersController> logger,
        OrderRepo repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var entities = await repo.GetAllAsync();

                return Ok(entities);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving Orders");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var entity = await repo.FindByIdAsync(id);
                if (entity == null)
                {
                    return NotFound();
                }
                return Ok(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving OrderID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync(OrderDto newOrder)
        {
            try
            {
                var entity = new OrderEntity
                {
                    OrderID = 0,
                    CustomerID = newOrder.CustomerID,
                    OrderDate = DateTime.UtcNow,
                    ShipAmount = newOrder.ShipAmount,
                    TaxAmount = newOrder.TaxAmount,
                    ShipDate = newOrder.ShipDate,
                    ShipAddressID = newOrder.ShipAddressID,
                    CardType = newOrder.CardType ?? string.Empty,
                    CardNumber = newOrder.CardNumber ?? string.Empty,
                    CardExpires = newOrder.CardExpires ?? string.Empty,
                    BillingAddressID = newOrder.BillingAddressID
                };

                var orderItemEntities = newOrder.OrderItems
                    .Select(dto => new OrderItemEntity()
                    {
                        ItemID = 0,
                        OrderID = 0,
                        ProductID = dto.ProductID!.Value,
                        ItemPrice = dto.ItemPrice,
                        DiscountAmount = dto.DiscountAmount,
                        Quantity = dto.Quantity
                    });

                var ordersCreated = await repo.InsertAsync(entity, orderItemEntities);

                return Ok($"{ordersCreated} new orders created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error adding new product");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderAsync(int id, OrderDto updatedOrder)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"OrderID {id} not found");

                var entity = new OrderEntity
                {
                    OrderID = 0,
                    CustomerID = updatedOrder.CustomerID,
                    OrderDate = updatedOrder.OrderDate,
                    ShipAmount = updatedOrder.ShipAmount,
                    TaxAmount = updatedOrder.TaxAmount,
                    ShipAddressID = updatedOrder.ShipAddressID,
                    CardType = updatedOrder.CardType ?? string.Empty,
                    CardNumber = updatedOrder.CardNumber ?? string.Empty,
                    CardExpires = updatedOrder.CardExpires ?? string.Empty,
                    BillingAddressID = updatedOrder.BillingAddressID
                };

                var updated = await repo.UpdateAsync(id, entity);

                return Ok($"{updated} order updated");
            }
            catch (Exception ex)
            {
                logger.LogError("Error updating OrderID {id}.\n\nError: {message}", id, ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"OrderID {id} not found");

                var deleted = await repo.DeleteAsync(id);

                return Ok($"{deleted} products deleted");
            }
            catch (Exception ex)
            {
                logger.LogError("Error deleting OrderID {id}\n\nError: {message}", id, ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
