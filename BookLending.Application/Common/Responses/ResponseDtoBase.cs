using BookLending.Domain.Enums;

namespace BookLending.Application.Common.Responses
{
    public abstract class ResponseDtoBase
    {
        public bool IsSuccess { get; set; }
        public ErrorType ErrorCode { get; set; }
        public string Message { get; set; }
    }
}