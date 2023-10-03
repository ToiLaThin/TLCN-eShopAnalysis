namespace eShopAnalysis.ProductCatalogAPI.Application.Result
{
    public enum ApiType
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public class BackChannelRequestDto<T> where T : class
    {
        public ApiType ApiType { get; set; } = ApiType.GET;

        public string Url { get; set; }

        public T Data { get; set; }

        public string AccessToken { get; set; }
    }
}
