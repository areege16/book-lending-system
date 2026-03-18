using BookLending.Domain.Enums;

namespace BookLending.Application.Common.Responses
{
    public class ResponseDto<T> : ResponseDtoBase
    {
        public T Data { get; set; }
        public static ResponseDto<T> Success(T data, string message = "")
        {
            return new ResponseDto<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                ErrorCode = ErrorType.None,
            };
        }
        public static ResponseDto<T> Error(ErrorType errorCode, string message = "")
        {
            return new ResponseDto<T>
            {
                Data = default,
                IsSuccess = false,
                Message = message,
                ErrorCode = errorCode,
            };
        }
    }
}