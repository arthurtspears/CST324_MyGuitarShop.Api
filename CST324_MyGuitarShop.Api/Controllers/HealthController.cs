using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyGuitarShop.Data.Ado.Factories;
using MyGuitarShop.Data.EFCore.Data;

namespace CST324_MyGuitarShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController(
        ILogger<HealthController> logger,
        SqlConnectionFactory sqlConnectionFactory,
        MyGuitarShopContext dbContext,
        IMongoClient mongoClient) 
        : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok("Healthy");
            }
            catch(Exception)
            {
                logger.LogWarning("Health check failed unreasonably");
                return StatusCode(503, "Unhealthy");
            }
        }

        [HttpGet("db/ado")]
        public IActionResult GetDbHealth()
        {
            try
            {
                using var connection = sqlConnectionFactory.OpenSqlConnection();

                return Ok(new{Message="Connection Successful!", connection.Database});
            }
            catch (Exception)
            {
                logger.LogCritical("Database health check failed");

                return StatusCode(503, "Database Unhealthy");
            }
        }

        [HttpGet("db/efcore")]
        public async Task<IActionResult> GetDbContextHealthAsync()
        {
            try
            {
                if(!await dbContext.Database.CanConnectAsync())
                    throw new Exception("Cannot connect to database via EFCore DbContext");

                return Ok(new{Message="EFCore DbContext Connection Successful!", dbContext.Database});
            }
            catch(Exception)
            {
                logger.LogCritical("EFCore DbContext health check failed");
                return StatusCode(503, "Database Unhealthy via EFCore DbContext");
            }
        }

        [HttpGet("db/mongo")]
        public async Task<IActionResult> GetMongoDbHealthAsync()
        {
            try
            {
                var response = await mongoClient.ListDatabaseNamesAsync();

                var databaseNames = await response.ToListAsync() ?? [];

                if (databaseNames.Count == 0)
                    throw new Exception("Cannot connect to Mongo database.");

                return Ok(new { Message = "Mongo Connection Successful!", databaseNames });
            }
            catch (Exception)
            {
                logger.LogCritical("MongoDb health check failed");

                return StatusCode(503, "Mongo Database connection unsuccessful.");
            }
        }
    }
}
