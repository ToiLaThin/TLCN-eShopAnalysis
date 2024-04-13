namespace eShopAnalysis.CartOrderAPI.Services.BackchannelDto
{
    /// <summary>
    /// This class wrap a primitive type data to become a reference type so as to send it via backChannel in the body of the request,
    /// request body cannot be a primitive type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReferenceTypeWrapperDto<T>
    {
        public T Data { get; set; }
        public ReferenceTypeWrapperDto() { }

        public ReferenceTypeWrapperDto(T primitiveTypeDate)
        {
            this.Data = primitiveTypeDate;
        }
    }
}
