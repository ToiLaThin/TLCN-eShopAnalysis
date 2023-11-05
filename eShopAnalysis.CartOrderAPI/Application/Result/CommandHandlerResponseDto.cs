namespace eShopAnalysis.CartOrderAPI.Application.Result
{
    public enum ResultType
    {
        Success,
        Failed,
        Exception
    }

    //even if this is CQRS, will still use ResponseDto in the handler
    public class CommandHandlerResponseDto<T> where T : class
    {
        //incapsulation can only be set through factory method
        public T Data { get; private set; } //jsonString Data

        public string Error { get; private set; }
        public ResultType Result { get; private set; }

        //read-only prop
        public bool IsSuccess => Result == ResultType.Success;

        public bool IsFailed => Result == ResultType.Failed;

        public bool IsException => Result == ResultType.Exception;



        private CommandHandlerResponseDto() { }

        //factory method to create an instance of that responseDto
        public static CommandHandlerResponseDto<T> Success(T result)
        {
            return new CommandHandlerResponseDto<T>
            {
                Data = result,
                Result = ResultType.Success
            };
        }

        public static CommandHandlerResponseDto<T> Failure(string errMessage)
        {
            return new CommandHandlerResponseDto<T>
            {
                Data = default(T),
                Result = ResultType.Failed,
                Error = errMessage
            };
        }

        public static CommandHandlerResponseDto<T> Exception(string exceptionMessage)
        {
            return new CommandHandlerResponseDto<T>
            {
                Data = default(T),
                Result = ResultType.Exception,
                Error = exceptionMessage
            };
        }
    }
}
