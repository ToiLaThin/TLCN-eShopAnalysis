namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Dto
{     
    //response for microservice communication
    public class BackChannelResponseDto<T> where T: class
    {
        public T Data { get; private set; }

        public string Error { get; private set; }

        public ResultType Result { get; private set; }

        public bool IsSuccess => Result == ResultType.Success;

        public bool IsFailed => Result == ResultType.Failed;

        public bool IsException => Result == ResultType.Exception;
        private BackChannelResponseDto() { }

        public static BackChannelResponseDto<T> Failure(string errMessage)
        {
            return new BackChannelResponseDto<T>
            {
                Error = errMessage,
                Data = default(T),
                Result = ResultType.Failed
            };
        }

        public static BackChannelResponseDto<T> Success(T result)
        {
            return new BackChannelResponseDto<T>
            {
                Data = result,
                Result = ResultType.Success
            };
        }

        public static BackChannelResponseDto<T> Exception(string exceptionMessage)
        {
            return new BackChannelResponseDto<T>
            {
                Error = exceptionMessage,
                Data = default(T),
                Result = ResultType.Exception
            };
        }
    }
}
