namespace eShopAnalysis.CartOrderAPI.Application.Result
{
    public class QueryResponseDto<T> where T : class
    {
        //incapsulation can only be set through factory method
        public T Data { get; private set; } //jsonString Data

        public string Error { get; private set; }
        public ResultType Result { get; private set; }

        //read-only prop
        public bool IsSuccess => Result == ResultType.Success;

        public bool IsFailed => Result == ResultType.Failed;

        public bool IsException => Result == ResultType.Exception;



        private QueryResponseDto() { }

        //factory method to create an instance of that responseDto
        public static QueryResponseDto<T> Success(T result)
        {
            return new QueryResponseDto<T>
            {
                Data = result,
                Result = ResultType.Success
            };
        }

        public static QueryResponseDto<T> Failure(string errMessage)
        {
            return new QueryResponseDto<T>
            {
                Data = default(T),
                Result = ResultType.Failed,
                Error = errMessage
            };
        }

        public static QueryResponseDto<T> Exception(string exceptionMessage)
        {
            return new QueryResponseDto<T>
            {
                Data = default(T),
                Result = ResultType.Exception,
                Error = exceptionMessage
            };
        }
    }
}
