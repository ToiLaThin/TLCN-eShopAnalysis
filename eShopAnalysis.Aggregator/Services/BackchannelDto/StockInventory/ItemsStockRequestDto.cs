using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    /// <summary>
    /// model này được dùng khi muốn lấy số lượng của các product models tương ứng.
    /// khi muốn lấy số lượng của các product models trong một order
    /// hay khi muốn lấy số lượng của các product model trong một provider requirement để aggregate thành một DTO khác khi lên UI
    /// .Lưu ý, nếu refactor name thì cần lưu ý là model này được duplicate ở service StockInventory, nên cũng nhớ thay đổi tên của nó ở StockInventory
    /// 
    /// request to aggregate to StockInventory
    /// in agregator read GetOrderToApproveWithStock
    /// </summary>
    public record ItemsStockRequestDto
    {
        //json prop is required for the model to be serialized or deserialized correctly
        //if not , there will be error
        [JsonProperty]
        IEnumerable<Guid> ProductModelIds { get; }

        [JsonConstructor]
        public ItemsStockRequestDto(IEnumerable<Guid> productModelIds)
        {
            this.ProductModelIds = productModelIds;
        }
    }
}
