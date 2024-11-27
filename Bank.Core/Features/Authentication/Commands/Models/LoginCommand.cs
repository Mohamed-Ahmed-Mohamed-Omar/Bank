using Bank.Core.Bases;
using Bank.Data.Helpers;
using MediatR;

namespace Bank.Core.Features.Authentication.Commands.Models
{
    public class LoginCommand : IRequest<Response<AuthModel>>
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
