using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Authentication.Commands.Models
{
    public class SendResetPasswordCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
    }
}
