using eShopAnalysis.CouponSaleItemAPI.Dto;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace eShopAnalysis.CouponSaleItemAPI.Service.BackChannelService
{
    //two generic arg is type of request and response, we will specify them in sub class, or in the call
    public class BackChannelBaseService<S, D> : IBackChannelBaseService<S, D> where S : class where D : class
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BackChannelBaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<BackChannelResponseDto<D>> SendAsync(BackChannelRequestDto<S> backChannelRequestDto)
        {
            try
            {

                HttpClient client = _httpClientFactory.CreateClient();
                HttpRequestMessage requestMsg = new();//this class to configure request header //what kind of ctor is this
                requestMsg.Headers.Add("Accept", "application/json");
                requestMsg.RequestUri = new Uri(backChannelRequestDto.Url);
                if (backChannelRequestDto.Data != null)
                {
                    requestMsg.Content = new StringContent(JsonConvert.SerializeObject(backChannelRequestDto.Data), Encoding.UTF8, "application/json");
                }
                switch (backChannelRequestDto.ApiType)
                {
                    case ApiType.POST:
                        requestMsg.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        requestMsg.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        requestMsg.Method = HttpMethod.Delete;
                        break;
                    default:
                        requestMsg.Method = HttpMethod.Get;
                        break;
                }

                var apiResponse = await client.SendAsync(requestMsg);
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return BackChannelResponseDto<D>.Failure("Not found");
                    case HttpStatusCode.Forbidden:
                        return BackChannelResponseDto<D>.Failure("Access denied");
                    case HttpStatusCode.Unauthorized:
                        return BackChannelResponseDto<D>.Failure("Unauthorized");
                    case HttpStatusCode.InternalServerError:
                        return BackChannelResponseDto<D>.Failure("Internal server error");
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<BackChannelResponseDto<D>>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var apiResponseDto = BackChannelResponseDto<D>.Exception(ex.Message);
                return apiResponseDto;
            }
        }
    }
}
