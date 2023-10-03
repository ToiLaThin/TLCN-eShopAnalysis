using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;
using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Utilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace eShopAnalysis.ProductCatalogAPI.Application.BackchannelServices
{
    public class BackChannelStockInventoryService : IBackChannelStockInventoryService
    {
        private IHttpClientFactory _httpClientFactory;
        public BackChannelStockInventoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<BackChannelResponseDto<StockInventoryDto>> AddNewStockInventory(string productId, string productModelId, string businessKey)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("StockInventoryAPI"); //client này sẽ có base address theo như config trong program.cs
                HttpRequestMessage requestMsg = new();
                requestMsg.Headers.Add("productId", productId);
                requestMsg.Headers.Add("productModelId", productModelId);
                requestMsg.Headers.Add("businessKey", businessKey);

                requestMsg.RequestUri = new Uri($"{httpClient.BaseAddress}/AddStock");

                requestMsg.Method = HttpMethod.Post;
                //add bearer token to header if require
                var apiResponse = await httpClient.SendAsync(requestMsg);
                switch(apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return BackChannelResponseDto<StockInventoryDto>.Failure("Not found");
                    case HttpStatusCode.Forbidden:
                        return BackChannelResponseDto<StockInventoryDto>.Failure("Access denied");
                    case HttpStatusCode.Unauthorized:
                        return BackChannelResponseDto<StockInventoryDto>.Failure("Unauthorized");
                    case HttpStatusCode.InternalServerError:
                        return BackChannelResponseDto<StockInventoryDto>.Failure("Internal server error");
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<BackChannelResponseDto<StockInventoryDto>>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                return BackChannelResponseDto<StockInventoryDto>.Exception(ex.Message);
            }
        }
    }
}
