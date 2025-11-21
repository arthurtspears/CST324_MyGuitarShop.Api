using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class ProductRepo(
        ILogger<ProductRepo> logger, 
        SqlConnectionFactory sqlConnectionFactory) 
        : IRepository<ProductEntity>
    {
        public async Task<IEnumerable<ProductEntity>> GetAllAsync()
        {
            var products = new List<ProductEntity>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand("SELECT * FROM Products", connection);

                await using var reader = await command.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    var product = new ProductEntity
                    {
                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        ProductCode = reader.GetString(reader.GetOrdinal("ProductCode")),
                        ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        ListPrice = reader.GetDecimal(reader.GetOrdinal("ListPrice")),
                        DiscountPercent = reader.GetDecimal(reader.GetOrdinal("DiscountPercent")),
                        DateAdded = reader.IsDBNull(reader.GetOrdinal("DateAdded")) ? null : reader.GetDateTime(reader.GetOrdinal("DateAdded"))
                    };
                    products.Add(product);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving product list");
            }
            return products;
        }

        public async Task<ProductEntity?> FindByIdAsync(int id)
        {
            ProductEntity? product = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand("SELECT * FROM Products WHERE ProductID = @ProductID", connection);

                command.Parameters.AddWithValue("@ProductID", id);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    product = new ProductEntity()
                    {
                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        ProductCode = reader.GetString(reader.GetOrdinal("ProductCode")),
                        ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        ListPrice = reader.GetDecimal(reader.GetOrdinal("ListPrice")),
                        DiscountPercent = reader.GetDecimal(reader.GetOrdinal("DiscountPercent")),
                        DateAdded = reader.IsDBNull(reader.GetOrdinal("DateAdded")) ? null : reader.GetDateTime(reader.GetOrdinal("DateAdded"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error finding product {id} by ID");
            }
            return product;
        }

        public async Task<int> InsertAsync(ProductEntity entity)
        {
            const string query = @"
                INSERT INTO Products (CategoryID, ProductCode, ProductName, Description, ListPrice, DiscountPercent, DateAdded)
                VALUES (@CategoryID, @ProductCode, @ProductName, @Description, @ListPrice, @DiscountPercent, @DateAdded);";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@CategoryID", entity.CategoryID);
                command.Parameters.AddWithValue("@ProductCode", entity.ProductCode);
                command.Parameters.AddWithValue("@ProductName", entity.ProductName);
                command.Parameters.AddWithValue("@Description", entity.Description);
                command.Parameters.AddWithValue("@ListPrice", entity.ListPrice);
                command.Parameters.AddWithValue("@DiscountPercent", entity.DiscountPercent);
                command.Parameters.AddWithValue("@DateAdded", DateTime.UtcNow);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new product");
                return 0;
            }
        }

        public async Task<int> UpdateAsync(int id, ProductEntity entity)
        {
            const string query = @"UPDATE Products
                                    SET CategoryID = @CategoryID, ProductCode = @ProductCode, ProductName = @ProductName, Description = @Description, ListPrice = @ListPrice, DiscountPercent = @DiscountPercent
                                    WHERE ProductID = @ProductID";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@CategoryID", entity.CategoryID);

                command.Parameters.AddWithValue("@ProductCode", entity.ProductCode);

                command.Parameters.AddWithValue("@ProductID", id);

                command.Parameters.AddWithValue("@ProductName", entity.ProductName);

                command.Parameters.AddWithValue("@Description", entity.Description);

                command.Parameters.AddWithValue("@ListPrice", entity.ListPrice);

                command.Parameters.AddWithValue("@DiscountPercent", entity.DiscountPercent);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error Updating Product");

                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string query = @"DELETE FROM Products WHERE ProductID = @ProductID";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ProductID", id);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error Deleting Product");

                return 0;
            }
        }
    }
}
