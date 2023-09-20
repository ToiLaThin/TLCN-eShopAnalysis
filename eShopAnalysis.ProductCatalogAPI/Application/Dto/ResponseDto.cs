namespace eShopAnalysis.ProductCatalogAPI.Application.Dto
{
    public class ResponseDto
    {
        public object Result { get; set; }
        //support boxing, unboxing any dataype
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
