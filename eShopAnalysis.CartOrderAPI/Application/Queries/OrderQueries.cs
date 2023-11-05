using Dapper;
using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Services.BackchannelDto;
using Microsoft.Data.SqlClient;
//using OrderStatus = eShopAnalysis.CartOrderAPI.Services.BackchannelDto.OrderStatus;

namespace eShopAnalysis.CartOrderAPI.Application.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private string _connString = String.Empty;

        public OrderQueries(string connString)
        {
            _connString = !string.IsNullOrWhiteSpace(connString) ? connString : throw new ArgumentNullException(nameof(connString));
        }

        //ref: https://www.learndapper.com/relationships
        public async Task<QueryResponseDto<IEnumerable<OrderItemsResponseDto>>> GetToApprovedOrders(int limit)
        {
            using var connection = new SqlConnection(_connString);
            string sql = @"SELECT o.Id AS OrderId, o.OrdersStatus AS OrderStatus, o.PaymentMethod, c.TotalPriceFinal As TotalPriceFinal, 
                                  cI.ProductModelId AS ProductModelId, cI.Quantity As Quantity
                           FROM Orders o
                           INNER JOIN Cart c on o.CartId = c.Id
                           INNER JOIN CartItem cI ON c.Id = cI.CartId   
                           WHERE o.OrdersStatus = @oStCheckedout OR ( o.OrdersStatus = @oStCustomerInfoConfirmed AND o.PaymentMethod = @pMthCOD)
                          ";
            object paramsSql = new { 
                oStCheckedout = OrderStatus.Checkouted,
                oStCustomerInfoConfirmed = OrderStatus.CustomerInfoConfirmed,
                pMthCOD = PaymentMethod.COD
            };
            //need to be group since now each orderItems is with a single ItemQty
            var orderItemsEntry = await connection.QueryAsync<OrderItemsResponseDto, OrderItemQuantityDto, OrderItemsResponseDto>(
                command: new CommandDefinition(sql,paramsSql), 
                map: (orderItems, itemQty) =>
                {
                    if (orderItems.OrderItemsQty == null) {
                        orderItems.OrderItemsQty = new List<OrderItemQuantityDto>();
                    }
                    orderItems.OrderItemsQty.Add(itemQty);
                    return orderItems;
                },
                splitOn: "ProductModelId");
            if (orderItemsEntry == null) {
                return QueryResponseDto<IEnumerable<OrderItemsResponseDto>>.Failure("Query return no result");
            }
            //group orderItems with same id and return a single orderItems with itemQty is list of all order of that group
            IEnumerable<OrderItemsResponseDto> orderItemsGrp = orderItemsEntry.GroupBy(o => o.OrderId).Select(g =>
            {
                //from group of orderItemsEntry, pickone with OrderItemsQty is List of 
                var groupedOrderItems = g.First();
                groupedOrderItems.OrderItemsQty = g.Select(g => g.OrderItemsQty.First()).ToList(); //necessary
                return groupedOrderItems;
            });
            return QueryResponseDto<IEnumerable<OrderItemsResponseDto>>.Success(orderItemsGrp);
        }

        public async Task<QueryResponseDto<IEnumerable<OrderDraftViewModel>>> GetUserDraftOrders(Guid userId)
        {
            using var connection = new SqlConnection(_connString);
            string sql = @"SELECT o.Id As OrderId, c.TotalPriceOriginal As SubTotal, c.CouponDiscountAmount + c.TotalSaleDiscountAmount As TotalDiscount
                           FROM Orders o
                           INNER JOIN Cart c ON o.CartId = c.Id 
                           WHERE c.UserId = @UserId AND o.OrdersStatus = @OrdersStatus";
            object paramsSql = new { userId, OrdersStatus = OrderStatus.CreatedDraft };

            var result = await connection.QueryAsync<OrderDraftViewModel>(sql, paramsSql);
            if (result == null || result.Count() <= 0) {
                return QueryResponseDto<IEnumerable<OrderDraftViewModel>>.Failure("Query return no result");
            }
            return QueryResponseDto<IEnumerable<OrderDraftViewModel>>.Success(result);

        }
    }
}
