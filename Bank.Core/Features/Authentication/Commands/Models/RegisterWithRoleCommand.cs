using Bank.Core.Bases;
using Bank.Data.Helpers;
using MediatR;

namespace Bank.Core.Features.Authentication.Commands.Models
{
    public class RegisterWithRoleCommand : IRequest<Response<AuthModel>>
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string CreditCardNumber { get; set; }

        public string? PhoneNumber { get; set; }

        public string RoleName { get; set; }
    }
}
