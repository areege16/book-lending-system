using BookLending.Application.Account.Login;
using BookLending.Application.Account.Register;
using BookLending.Application.DTOs.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookLending.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {

            var result = await _mediator.Send(new RegisterCommand
            (
                registerRequestDto
            ));
            return Ok(result);
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var result = await _mediator.Send(new LoginCommand
            (
                loginRequestDto
            ));
            return Ok(result);
        }
        #endregion

    }
}