using Dapper;
using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
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

        public async Task<QueryResponseDto<OrderAggregateCartViewModel>> GetOrderAggregateCartByCartIdWithoutAddressUsingRelationship(Guid cartId)
        {
            using var connection = new SqlConnection(_connString);
            string sql = @"SELECT o.Id As OrderId, o.CartId, o.BusinessKey AS OrderBusinessKey, o.OrdersStatus AS OrderStatus, o.PaymentMethod, o.PhoneNumber, 
                                  o.DateCreatedDraft, o.DateCustomerInfoConfirmed, o.DateCheckouted, o.DateStockConfirmed, o.DateRefunded, o.DateCancelled, o.DateCompleted,
                                    
                                  c.Id AS CartPrimaryKey, c.UserId, c.HaveCouponApplied, c.CouponId, c.HaveAnySaleItem, c.CouponDiscountType,
                                  c.CouponDiscountAmount, c.CouponDiscountValue, c.TotalSaleDiscountAmount, c.TotalPriceOriginal, c.TotalPriceAfterSale, c.TotalPriceAfterCouponApplied,
                                  c.TotalPriceFinal,

                                  cI.ProductId, cI.ProductModelId, cI.BusinessKey AS CartItemBusinessKey, cI.CartId, cI.SaleItemId, cI.IsOnSale, cI.SaleType, cI.SaleValue,
                                  cI.Quantity, cI.UnitPrice, cI.FinalPrice, cI.UnitAfterSalePrice, cI.FinalAfterSalePrice, cI.ProductName, cI.ProductImage, cI.SubCatalogName
                           FROM Orders o            
                           INNER JOIN Cart c on o.CartId = c.Id
                           INNER JOIN CartItem cI ON c.Id = cI.CartId
                           WHERE o.CartId = @cartId
                         ";
            object paramsSql = new
            {
                cartId = cartId
            };
            var orderAggregateCartEntries = await connection.QueryAsync<OrderAggregateCartViewModel, CartSummaryViewModel, CartItemViewModel, OrderAggregateCartViewModel>(
                sql,
                map: (order, cart, cartItem) =>
                {
                    cart.Items = new List<CartItemViewModel> { cartItem };
                    order.Cart = cart;
                    return order;
                },
                param: paramsSql,
                splitOn: "CartPrimaryKey, ProductId");
            if (orderAggregateCartEntries == null)
            {
                return QueryResponseDto<OrderAggregateCartViewModel>.Failure("Query return no result");
            }

            var orderAggregateCartGroups = orderAggregateCartEntries.GroupBy(oC => new { oC.OrderId, oC.CartId }); //key: {orderId & cartId}, value is list of OrderAggregateCartViewModel
            var orderAggregateCartGroupsWithMutliItems = orderAggregateCartGroups.Select(g =>
            {
                var groupedOrderAggregateCart = g.First();
                groupedOrderAggregateCart.Cart.Items = g.Select(g => g.Cart.Items.First()).ToList(); //get all first cartItem from the list OrderAggregateCartViewModel and concat to a single list, assign for Items in the first OrderAggregateCartViewModel
                return groupedOrderAggregateCart;
            });
            var orderAggregateCartOneGroupWithMutliItems = orderAggregateCartGroupsWithMutliItems.FirstOrDefault();
            if (orderAggregateCartOneGroupWithMutliItems == null)
            {
                return QueryResponseDto<OrderAggregateCartViewModel>.Failure("Query return no result");
            }
            return QueryResponseDto<OrderAggregateCartViewModel>.Success(orderAggregateCartOneGroupWithMutliItems);
        }

        public async Task<QueryResponseDto<OrderAggregateCartViewModel>> GetOrderAggregateCartByCartIdUsingRelationship(Guid cartId)
        {
            using var connection = new SqlConnection(_connString);
            string sql = @"SELECT o.Id As OrderId, o.CartId, o.BusinessKey AS OrderBusinessKey, o.OrdersStatus AS OrderStatus, o.PaymentMethod, o.PhoneNumber, 
                                  o.DateCreatedDraft, o.DateCustomerInfoConfirmed, o.DateCheckouted, o.DateStockConfirmed, o.DateRefunded, o.DateCancelled, o.DateCompleted,
                                    
                                  a.OrderId AS OrderReferenceId, a.Country, a.CityOrProvinceOrPlace, a.DistrictOrLocality, a.PostalCode, a.Street, a.FullAddressName,

                                  c.Id AS CartPrimaryKey, c.UserId, c.HaveCouponApplied, c.CouponId, c.HaveAnySaleItem, c.CouponDiscountType,
                                  c.CouponDiscountAmount, c.CouponDiscountValue, c.TotalSaleDiscountAmount, c.TotalPriceOriginal, c.TotalPriceAfterSale, c.TotalPriceAfterCouponApplied,
                                  c.TotalPriceFinal,

                                  cI.ProductId, cI.ProductModelId, cI.BusinessKey AS CartItemBusinessKey, cI.CartId, cI.SaleItemId, cI.IsOnSale, cI.SaleType, cI.SaleValue,
                                  cI.Quantity, cI.UnitPrice, cI.FinalPrice, cI.UnitAfterSalePrice, cI.FinalAfterSalePrice
                           FROM Orders o            
                           INNER JOIN Address a ON o.Id = a.OrderId
                           INNER JOIN Cart c on o.CartId = c.Id
                           INNER JOIN CartItem cI ON c.Id = cI.CartId
                           WHERE o.CartId = @cartId
                         ";
            object paramsSql = new {
                cartId = cartId
            };
            var orderAggregateCartEntries = await connection.QueryAsync<OrderAggregateCartViewModel, AddressViewModel, CartSummaryViewModel, CartItemViewModel, OrderAggregateCartViewModel>(
                sql,
                map:(order, address, cart, cartItem) =>
                {
                    cart.Items = new List<CartItemViewModel> { cartItem };
                    order.Cart = cart;
                    order.Address = address;
                    return order;
                },
                param: paramsSql,
                splitOn: "OrderReferenceId, CartPrimaryKey, ProductId");
            if (orderAggregateCartEntries == null)
            {
                return QueryResponseDto<OrderAggregateCartViewModel>.Failure("Query return no result");
            }

            var orderAggregateCartGroups = orderAggregateCartEntries.GroupBy(oC => new { oC.OrderId, oC.CartId }); //key: {orderId & cartId}, value is list of OrderAggregateCartViewModel
            var orderAggregateCartGroupsWithMutliItems = orderAggregateCartGroups.Select(g =>
            {
                var groupedOrderAggregateCart = g.First();
                groupedOrderAggregateCart.Cart.Items = g.Select(g => g.Cart.Items.First()).ToList(); //get all first cartItem from the list OrderAggregateCartViewModel and concat to a single list, assign for Items in the first OrderAggregateCartViewModel
                return groupedOrderAggregateCart;
            });
            var orderAggregateCartOneGroupWithMutliItems = orderAggregateCartGroupsWithMutliItems.FirstOrDefault();
            if (orderAggregateCartOneGroupWithMutliItems == null) {
                return QueryResponseDto<OrderAggregateCartViewModel>.Failure("Query return no result");
            }
            return QueryResponseDto<OrderAggregateCartViewModel>.Success(orderAggregateCartOneGroupWithMutliItems);
        }

        public async Task<QueryResponseDto<OrderAggregateCartViewModel>> GetOrderAggregateCartByOrderIdUsingRelationship(Guid orderId)
        {
            using var connection = new SqlConnection(_connString);
            string sql = @"SELECT o.Id As OrderId, o.CartId, o.BusinessKey AS OrderBusinessKey, o.OrdersStatus AS OrderStatus, o.PaymentMethod, o.PhoneNumber, 
                                  o.DateCreatedDraft, o.DateCustomerInfoConfirmed, o.DateCheckouted, o.DateStockConfirmed, o.DateRefunded, o.DateCancelled, o.DateCompleted,
                                    
                                  a.OrderId AS OrderReferenceId, a.Country, a.CityOrProvinceOrPlace, a.DistrictOrLocality, a.PostalCode, a.Street, a.FullAddressName,

                                  c.Id AS CartPrimaryKey, c.UserId, c.HaveCouponApplied, c.CouponId, c.HaveAnySaleItem, c.CouponDiscountType,
                                  c.CouponDiscountAmount, c.CouponDiscountValue, c.TotalSaleDiscountAmount, c.TotalPriceOriginal, c.TotalPriceAfterSale, c.TotalPriceAfterCouponApplied,
                                  c.TotalPriceFinal,

                                  cI.ProductId, cI.ProductModelId, cI.BusinessKey AS CartItemBusinessKey, cI.CartId, cI.SaleItemId, cI.IsOnSale, cI.SaleType, cI.SaleValue,
                                  cI.Quantity, cI.UnitPrice, cI.FinalPrice, cI.UnitAfterSalePrice, cI.FinalAfterSalePrice, cI.ProductName, cI.ProductImage, cI.SubCatalogName
                           FROM Orders o            
                           INNER JOIN Address a ON o.Id = a.OrderId
                           INNER JOIN Cart c on o.CartId = c.Id
                           INNER JOIN CartItem cI ON c.Id = cI.CartId
                           WHERE o.Id = @orderId
                         ";
            object paramsSql = new
            {
                orderId = orderId
            };
            var orderAggregateCartEntries = await connection.QueryAsync<OrderAggregateCartViewModel, AddressViewModel, CartSummaryViewModel, CartItemViewModel, OrderAggregateCartViewModel>(
                sql,
                map: (order, address, cart, cartItem) =>
                {
                    cart.Items = new List<CartItemViewModel> { cartItem };
                    order.Cart = cart;
                    order.Address = address;
                    return order;
                },
                param: paramsSql,
                splitOn: "OrderReferenceId, CartPrimaryKey, ProductId");
            if (orderAggregateCartEntries == null)
            {
                return QueryResponseDto<OrderAggregateCartViewModel>.Failure("Query return no result");
            }

            var orderAggregateCartGroups = orderAggregateCartEntries.GroupBy(oC => new { oC.OrderId, oC.CartId }); //key: {orderId & cartId}, value is list of OrderAggregateCartViewModel
            var orderAggregateCartGroupsWithMutliItems = orderAggregateCartGroups.Select(g =>
            {
                var groupedOrderAggregateCart = g.First();
                groupedOrderAggregateCart.Cart.Items = g.Select(g => g.Cart.Items.First()).ToList(); //get all first cartItem from the list OrderAggregateCartViewModel and concat to a single list, assign for Items in the first OrderAggregateCartViewModel
                return groupedOrderAggregateCart;
            });
            var orderAggregateCartOneGroupWithMutliItems = orderAggregateCartGroupsWithMutliItems.FirstOrDefault();
            if (orderAggregateCartOneGroupWithMutliItems == null)
            {
                return QueryResponseDto<OrderAggregateCartViewModel>.Failure("Query return no result");
            }
            return QueryResponseDto<OrderAggregateCartViewModel>.Success(orderAggregateCartOneGroupWithMutliItems);
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

        //no limit or offset but with the same where
        public async Task<int> GetOrdersAggregateCartTotalCountAfterFiletered(OrderStatus filterOrderStatus, PaymentMethod filterPaymentMethod)
        {
            using var connection = new SqlConnection(_connString);
            var builder = new SqlBuilder();

            //keep join clause with cart, so the number of return order is right like GetOrdersAggregateCartFilterSortPagination
            string sql = @"SELECT COUNT(*) 
                           FROM Orders o            
                           INNER JOIN Cart c on o.CartId = c.Id
                           /**where**/";
            object paramsSql = new
            {
                orderStatus = (int)filterOrderStatus,
                paymentMethod = (int)filterPaymentMethod,
            };

            SqlBuilder.Template sqlTemplate = builder.AddTemplate(sql);
            //we must check with a special value not in enum, since if nullable enum, we cannot access it by variable name
            if ((int)filterOrderStatus != -1) {
                builder.Where("o.OrdersStatus = @orderStatus");
            }

            //***avoid PaymentMethod col  have null value => cause result the be empty***
            if ((int)filterPaymentMethod != -1 && filterOrderStatus != OrderStatus.CreatedDraft && filterOrderStatus != OrderStatus.CustomerInfoConfirmed) {
                builder.Where("o.PaymentMethod = @paymentMethod");
            }
            var totalOrdersAfterFilterCount = await connection.ExecuteScalarAsync<int>(sqlTemplate.RawSql,param: paramsSql);
            return totalOrdersAfterFilterCount;
        }

        public async Task<QueryResponseDto<IEnumerable<OrderAggregateCartViewModel>>> GetOrdersAggregateCartFilterSortPagination(
            OrderStatus filterOrderStatus,
            PaymentMethod filterPaymentMethod,
            OrdersSortBy sortBy = OrdersSortBy.Id,
            int page = 1,
            int pageSize = 10,
            OrdersSortType sortType = OrdersSortType.Ascending)
        {
            using var connection = new SqlConnection(_connString);
            var builder = new SqlBuilder();
            string sql = @"SELECT o.Id As OrderId, o.CartId, o.BusinessKey AS OrderBusinessKey, o.OrdersStatus AS OrderStatus, o.PaymentMethod, o.PhoneNumber, 
                                  o.DateCreatedDraft, o.DateCustomerInfoConfirmed, o.DateCheckouted, o.DateStockConfirmed, o.DateRefunded, o.DateCancelled, o.DateCompleted,
                                    
                                  c.Id AS CartPrimaryKey, c.UserId, c.HaveCouponApplied, c.CouponId, c.HaveAnySaleItem, c.CouponDiscountType,
                                  c.CouponDiscountAmount, c.CouponDiscountValue, c.TotalSaleDiscountAmount, c.TotalPriceOriginal, c.TotalPriceAfterSale, c.TotalPriceAfterCouponApplied,
                                  c.TotalPriceFinal,

                                  cI.ProductId, cI.ProductModelId, cI.BusinessKey AS CartItemBusinessKey, cI.CartId, cI.SaleItemId, cI.IsOnSale, cI.SaleType, cI.SaleValue,
                                  cI.Quantity, cI.UnitPrice, cI.FinalPrice, cI.UnitAfterSalePrice, cI.FinalAfterSalePrice
                           FROM Orders o            
                           INNER JOIN Cart c on o.CartId = c.Id
                           INNER JOIN CartItem cI ON c.Id = cI.CartId
                           /**where**/ /**orderby**/  
                         ";
            //OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;
            //****this is not putted here, we want to skip and take after the orders, not the order join with cart item, might return the wrong result
            string sortTypeStr = sortType == OrdersSortType.Ascending ? "ASC" : "DESC";
            object paramsSql = new
            {
                //limit = pageSize,
                //offset = (page - 1) * pageSize,
                orderStatus = (int)filterOrderStatus,
                paymentMethod = (int)filterPaymentMethod,
            };
            //page num is 1 => offset = 0, page num is 2 => page idx = 1 => offset = 1 * pageSize 

            SqlBuilder.Template sqlTemplate = builder.AddTemplate(sql);
            //we must check with a special value not in enum, since if nullable enum, we cannot access it by variable name
            if ((int)filterOrderStatus != -1)
            {
                builder.Where("o.OrdersStatus = @orderStatus");
            }

            //***avoid PaymentMethod col  have null value => cause result the be empty***
            if ((int)filterPaymentMethod != -1 && filterOrderStatus != OrderStatus.CreatedDraft && filterOrderStatus != OrderStatus.CustomerInfoConfirmed)
            {
                builder.Where("o.PaymentMethod = @paymentMethod");
            }
            if (sortBy != null)
            {
                if (sortBy == OrdersSortBy.SubTotal)
                {
                    builder.OrderBy("TotalPriceFinal " + sortTypeStr); //must have ASC OR DESC in OrderBy, not by dynamic params
                }
                if (sortBy == OrdersSortBy.DateCreatedDraft)
                {
                    builder.OrderBy("DateCreatedDraft " + sortTypeStr); //also we cannot use parameterized column here
                }
                if (sortBy == OrdersSortBy.Id)
                {
                    builder.OrderBy("o.Id " + sortTypeStr); //must have orderby to have limit offset
                }
            }

            var orderAggregateCartEntries = await connection.QueryAsync<OrderAggregateCartViewModel, CartSummaryViewModel, CartItemViewModel, OrderAggregateCartViewModel>(
                sqlTemplate.RawSql,
                map: (order, cart, cartItem) =>
                {
                    cart.Items = new List<CartItemViewModel> { cartItem };
                    order.Cart = cart;
                    return order;
                },
                param: paramsSql,
                splitOn: "CartPrimaryKey, ProductId");

            if (orderAggregateCartEntries == null)
            {
                return QueryResponseDto<IEnumerable<OrderAggregateCartViewModel>>.Failure("Query return no result");
            }

            var orderAggregateCartGroups = orderAggregateCartEntries.GroupBy(oC => new { oC.OrderId, oC.CartId }); //key: {orderId & cartId}, value is list of OrderAggregateCartViewModel
            var orderAggregateCartGroupsWithMutliItems = orderAggregateCartGroups.Select(g =>
            {
                var groupedOrderAggregateCart = g.First();
                groupedOrderAggregateCart.Cart.Items = g.Select(g => g.Cart.Items.First()).ToList(); //get all first cartItem from the list OrderAggregateCartViewModel and concat to a single list, assign for Items in the first OrderAggregateCartViewModel
                return groupedOrderAggregateCart;
            });
            if (orderAggregateCartGroupsWithMutliItems == null)
            {
                return QueryResponseDto<IEnumerable<OrderAggregateCartViewModel>>.Failure("Query return no result");
            }
            var paginatedOrderAggregateCartGroupsWithMutliItems = orderAggregateCartGroupsWithMutliItems.Skip((page - 1) * pageSize)
                                                                                                        .Take(pageSize);
            return QueryResponseDto<IEnumerable<OrderAggregateCartViewModel>>.Success(paginatedOrderAggregateCartGroupsWithMutliItems);
        }        
    }
}
