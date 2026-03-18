using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Account;
using MediatR;

namespace BookLending.Application.Account.Register
{
    public record RegisterCommand(RegisterRequestDto RegisterRequestDto) : IRequest<ResponseDto<bool>>;
}
