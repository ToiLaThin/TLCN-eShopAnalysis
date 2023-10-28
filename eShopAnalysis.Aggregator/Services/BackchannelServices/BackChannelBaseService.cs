using eShopAnalysis.Aggregator.Result;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
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
                        var apiResponseDtoJObject = JsonConvert.DeserializeObject(apiContent) as JObject;
                        //jsonConvert đã sai, luôn deserialize ră sai Deserialize<BackchannelResponseDto<D>> do cai type D, thu serialize thanh string truoc khi gui no di => ko duoc, doc sai
                        //o các controller action backchannel
                        //solution la chuyen sang JObject  rồi tự construct nên BackChannelResponseDto<D> qua JOject to LinQ
                        //refer to this https://stackoverflow.com/questions/44545955/generic-type-jsonconvert-deserializeobjectlisttstring
                        //and this: https://stackoverflow.com/questions/25672338/dynamically-deserialize-json-into-any-object-passed-in-c-sharp
                        var apiResponseDto = ConvertJObjToGenericBackChannelResponseDto(apiResponseDtoJObject);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var apiResponseDto = BackChannelResponseDto<D>.Exception(ex.Message);
                return apiResponseDto;
            }
        }

        private static BackChannelResponseDto<D> ConvertJObjToGenericBackChannelResponseDto(JObject jObj)
        {
            //keys duoc camelCase
            string dataKey = "data";
            string resultKey = "result";
            string errorKey = "error"; //for both failure and exception
            ResultType resultType = jObj[resultKey].ToObject<ResultType>();
            switch (resultType)
            {
                case ResultType.Success:
                    var adapterResult = BackChannelResponseDto<D>.Success(jObj[dataKey].ToObject<D>());
                    return adapterResult;
                case ResultType.Failed:
                    adapterResult = BackChannelResponseDto<D>.Failure(jObj[errorKey].ToString());
                    return adapterResult;
                case ResultType.Exception:
                    adapterResult = BackChannelResponseDto<D>.Exception(jObj[errorKey].ToString());
                    return adapterResult;
                default:
                    return BackChannelResponseDto<D>.Failure("unknown resultType");
            }
        }
    }
}
