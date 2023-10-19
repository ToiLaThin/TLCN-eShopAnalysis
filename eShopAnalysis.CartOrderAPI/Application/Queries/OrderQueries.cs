using Dapper;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using Microsoft.Data.SqlClient;

namespace eShopAnalysis.CartOrderAPI.Application.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private string _connString = String.Empty;

        public OrderQueries(string connString)
        {
            _connString = !string.IsNullOrWhiteSpace(connString) ? connString : throw new ArgumentNullException(nameof(connString));
        }
        public async Task<IEnumerable<OrderDraftViewModel>> GetUserDraftOrders(Guid userId)
        {
            using var connection = new SqlConnection(_connString);
            string sql = @"SELECT o.Id As OrderId
                           FROM Orders o
                           INNER JOIN Cart c ON o.CartId = c.Id 
                           WHERE c.UserId = @UserId AND o.OrdersStatus = @OrdersStatus";
            object paramsSql = new { userId, OrdersStatus = OrderStatus.CreatedDraft };

            var result = await connection.QueryAsync<OrderDraftViewModel>(sql, paramsSql);
            return result;
        }
    }
}
