namespace eShopAnalysis.ProductCatalogAPI.Application.Result
{
    //generic response so that controller do not have to validate from service
    //it is self contained in this object

    public enum ResultType
    {
        Success,
        Failed,
        Exception
    }

    //this response dto is used between service and controller to handler the result of business logic
    //this response dto should not be used between controller to communicate to client, because they will have to decode it many time
    //this response dto should be used after get data from http client to other microservices

    //operation response pattern
    public class ServiceResponseDto<T> where T : class
    {
        //incapsulation can only be set through factory method
        public T Data { get; private set; } //jsonString Data

        public string Error { get; private set; }
        public ResultType Result { get; private set; }

        //read-only prop
        public bool IsSuccess => Result == ResultType.Success;
        
        public bool IsFailed => Result == ResultType.Failed;

        public bool IsException => Result == ResultType.Exception;



        private ServiceResponseDto() { }

        //factory method to create an instance of that responseDto
        public static ServiceResponseDto<T> Success(T result)
        {
            return new ServiceResponseDto<T>
            {
                Data = result,
                Result = ResultType.Success
            };
        }

        public static ServiceResponseDto<T> Failure(string errMessage)
        {
            return new ServiceResponseDto<T>
            {
                Data = default(T),
                Result = ResultType.Failed,
                Error = errMessage
            };
        }

        public static ServiceResponseDto<T> Exception(string exceptionMessage)
        {
            return new ServiceResponseDto<T>
            {   
                Data = default(T),
                Result = ResultType.Exception,
                Error = exceptionMessage
            };
        }
    }
}
