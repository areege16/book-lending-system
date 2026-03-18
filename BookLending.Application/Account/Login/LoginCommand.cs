using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Account;
using MediatR;

namespace BookLending.Application.Account.Login
{
    public record LoginCommand(LoginRequestDto LoginRequestDto) : IRequest<ResponseDto<LoginResponseDto>>;
}