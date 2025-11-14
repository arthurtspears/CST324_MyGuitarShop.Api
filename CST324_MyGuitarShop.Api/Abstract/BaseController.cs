using CST324_MyGuitarShop.Api.Mappers;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Interfaces;

namespace CST324_MyGuitarShop.Api.Abstract
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TDto, TEntity>(
        IRepository<TEntity> repo,
        ILogger<BaseController<TDto, TEntity>> logger
        ) : ControllerBase
        where TEntity : class, new()
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var entities = await repo.GetAllAsync();

                return entities.Any() ? Ok(entities) : NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving entities");
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
                    return NotFound($"Entity with id {id} not found");
                }
                return Ok(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving entity with ID {EntityID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TDto dto)
        {
            try
            {
                var entity = AutoReflectionMapper.Map<TDto, TEntity>(dto);

                if (entity == null)
                {
                    throw new Exception("Mapping resulted in null entity");
                }

                var entitiesCreated = await repo.InsertAsync(entity);

                return Ok($"{entitiesCreated} new entities created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating new entity");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, TDto dto)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Entity with id {id} not found");

                var entity = AutoReflectionMapper.Map<TDto, TEntity>(dto)
                    ?? throw new Exception("Mapping resulted in null entity");

                var entitiesUpdated = await repo.UpdateAsync(id, entity);

                return Ok($"{entitiesUpdated} entities updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating entity with ID {EntityID}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Entity with id {id} not found");

                var entitiesDeleted = await repo.DeleteAsync(id);

                return Ok($"{entitiesDeleted} entities deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting entity with ID {EntityID}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
