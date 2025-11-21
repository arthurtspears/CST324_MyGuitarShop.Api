using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;

namespace CST324_MyGuitarShop.Api.Controllers.AdoControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(
        ILogger<ProductsController> logger,
        IRepository<ProductEntity> repo) 
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var products = await repo.GetAllAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving products");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var product = await repo.FindByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving product with ID {ProductID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(ProductDto newProduct)
        {
            try
            {
                var entity = new ProductEntity
                {
                    ProductID = 0,
                    ProductCode = newProduct.ProductCode,
                    ProductName = newProduct.ProductName,
                    Description = newProduct.Description,
                    ListPrice = newProduct.ListPrice,
                    DiscountPercent = newProduct.DiscountPercent
                };

                var numberProductsCreated = await repo.InsertAsync(entity);

                return Ok($"{numberProductsCreated} new products created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error adding new product");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, ProductDto updatedProduct)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Product with id {id} not found");

                var entity = new ProductEntity
                {
                    ProductID = 0,
                    ProductCode = updatedProduct.ProductCode,
                    ProductName = updatedProduct.ProductName,
                    Description = updatedProduct.Description,
                    ListPrice = updatedProduct.ListPrice,
                    DiscountPercent = updatedProduct.DiscountPercent
                };

                var numberProductsUpdated = await repo.UpdateAsync(id, entity);

                return Ok($"{numberProductsUpdated} products updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error updating product with ID {ProductID}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Product with id {id} not found");

                var numberProductsDeleted = await repo.DeleteAsync(id);

                return Ok($"{numberProductsDeleted} products deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error deleting product with ID {ProductID}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
