using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class OrderRepo(
        ILogger<OrderRepo> logger, 
        SqlConnectionFactory sqlConnectionFactory) 
        : IRepository<OrderEntity>
    {
        public async Task<IEnumerable<OrderEntity>> GetAllAsync()
        {
            var orders = new List<OrderEntity>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand("SELECT * FROM Orders", connection);

                await using var reader = await command.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    var entity = new OrderEntity
                    {
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                        ShipAmount = reader.GetDecimal(reader.GetOrdinal("ShipAmount")),
                        TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                        ShipAddressID = reader.GetInt32(reader.GetOrdinal("ShipAddressID")),
                        CardType = reader.GetString(reader.GetOrdinal("CardType")),
                        CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                        CardExpires = reader.GetString(reader.GetOrdinal("CardExpires")),
                        BillingAddressID = reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                    orders.Add(entity);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error retrieving Order list.\n\n{msg}", ex.Message);
            }
            return orders;
        }

        public async Task<OrderEntity?> FindByIdAsync(int id)
        {
            OrderEntity? entity = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand("SELECT * FROM Orders WHERE OrderID = @OrderID", connection);

                command.Parameters.AddWithValue("@OrderID", id);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    entity = new OrderEntity
                    {
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                        ShipAmount = reader.GetDecimal(reader.GetOrdinal("ShipAmount")),
                        TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                        ShipAddressID = reader.GetInt32(reader.GetOrdinal("ShipAddressID")),
                        CardType = reader.GetString(reader.GetOrdinal("CardType")),
                        CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                        CardExpires = reader.GetString(reader.GetOrdinal("CardExpires")),
                        BillingAddressID = reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error finding order {id} by ID.\n\n{msg}", id, ex.Message);
            }
            return entity;
        }

        public async Task<int> InsertAsync(OrderEntity entity)
        {
            const string query = @"
                INSERT INTO Orders (CustomerID, OrderDate, ShipAmount, TaxAmount, ShipDate, ShipAddressID, CardType, CardNumber, CardExpires, BillingAddressID)
                VALUES (@CustomerID, @OrderDate, @ShipAmount, @TaxAmount, @ShipDate, @ShipAddressID, @CardType, @CardNumber, @CardExpires, @BillingAddressID);";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@CustomerID", entity.CustomerID);
                command.Parameters.AddWithValue("@OrderDate", entity.OrderDate);
                command.Parameters.AddWithValue("@ShipAmount", entity.ShipAmount);
                command.Parameters.AddWithValue("@TaxAmount", entity.TaxAmount);
                command.Parameters.AddWithValue("@ShipDate", entity.ShipDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShipAddressID", entity.ShipAddressID);
                command.Parameters.AddWithValue("@CardType", entity.CardType);
                command.Parameters.AddWithValue("@CardNumber", entity.CardNumber);
                command.Parameters.AddWithValue("@CardExpires", entity.CardExpires);
                command.Parameters.AddWithValue("@BillingAddressID", entity.BillingAddressID);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Error Inserting new Order.\nError:\n\n{message}", ex.Message);
                return 0;
            }
        }

        public async Task<int> InsertAsync(OrderEntity entity, IEnumerable<OrderItemEntity> orderItems)
        {
            const string insertOrder = @"
                INSERT INTO Orders (CustomerID, OrderDate, ShipAmount, TaxAmount, ShipDate, ShipAddressID, CardType, CardNumber, CardExpires, BillingAddressID)
                OUTPUT INSERTED.OrderID
                VALUES (@CustomerID, @OrderDate, @ShipAmount, @TaxAmount, @ShipDate, @ShipAddressID, @CardType, @CardNumber, @CardExpires, @BillingAddressID);";

            const string insertOrderItem = @"
                INSERT INTO OrderItems (OrderID, ProductID, ItemPrice, DiscountAmount, Quantity)
                VALUES (@OrderID, @ProductID, @ItemPrice, @DiscountAmount, @Quantity);";

            await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

            await using var transaction = connection.BeginTransaction();

            await using var command = new SqlCommand(insertOrder, connection, transaction);

            try
            {
                command.Parameters.AddWithValue("@CustomerID", entity.CustomerID);
                command.Parameters.AddWithValue("@OrderDate", entity.OrderDate);
                command.Parameters.AddWithValue("@ShipAmount", entity.ShipAmount);
                command.Parameters.AddWithValue("@TaxAmount", entity.TaxAmount);
                command.Parameters.AddWithValue("@ShipDate", entity.ShipDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShipAddressID", entity.ShipAddressID);
                command.Parameters.AddWithValue("@CardType", entity.CardType);
                command.Parameters.AddWithValue("@CardNumber", entity.CardNumber);
                command.Parameters.AddWithValue("@CardExpires", entity.CardExpires);
                command.Parameters.AddWithValue("@BillingAddressID", entity.BillingAddressID);
                
                // This returns the OrderID from the OUTPUT clause
                var result = await command.ExecuteScalarAsync();
                var orderId = Convert.ToInt32(result);

                foreach (var itemEntity in orderItems)
                {
                    await using var itemCommand = new SqlCommand(insertOrderItem, connection, transaction);

                    itemCommand.Parameters.AddWithValue("@OrderID", orderId);
                    itemCommand.Parameters.AddWithValue("@ProductID", itemEntity.ProductID);
                    itemCommand.Parameters.AddWithValue("@ItemPrice", itemEntity.ItemPrice);
                    itemCommand.Parameters.AddWithValue("@DiscountAmount", itemEntity.DiscountAmount);
                    itemCommand.Parameters.AddWithValue("@Quantity", itemEntity.Quantity);

                    await itemCommand.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();

                return orderId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                logger.LogError("Error Inserting new Order.\nError:\n\n{message}", ex.Message);
                return 0;
            }
        }

        public async Task<int> UpdateAsync(int id, OrderEntity entity)
        {
            const string query = @"UPDATE Orders
                                    SET CustomerID = @CustomerID, OrderDate = @OrderDate, ShipAmount = @ShipAmount, TaxAmount = @TaxAmount, ShipDate = @ShipDate, ShipAddressID = @ShipAddressID, CardType = @CardType, CardNumber = @CardNumber, CardExpires = @CardExpires, BillingAddressID = @BillingAddressID
                                    WHERE OrderID = @OrderID";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@CustomerID", entity.CustomerID);
                command.Parameters.AddWithValue("@OrderDate", entity.OrderDate);
                command.Parameters.AddWithValue("@ShipAmount", entity.ShipAmount);
                command.Parameters.AddWithValue("@TaxAmount", entity.TaxAmount);
                command.Parameters.AddWithValue("@ShipDate", entity.ShipDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShipAddressID", entity.ShipAddressID);
                command.Parameters.AddWithValue("@CardType", entity.CardType);
                command.Parameters.AddWithValue("@CardNumber", entity.CardNumber);
                command.Parameters.AddWithValue("@CardExpires", entity.CardExpires);
                command.Parameters.AddWithValue("@BillingAddressID", entity.BillingAddressID);

                command.Parameters.AddWithValue("@OrderID", id);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Error updating OrderID {id}.\nError:\n\n{message}", id, ex.Message);

                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string query = @"DELETE FROM Orders WHERE OrderID = @OrderID";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                await using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@OrderID", id);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Error Deleting OrderID {id}.\nError:\n\n{message}", id, ex.Message);

                return 0;
            }
        }
    }
}
