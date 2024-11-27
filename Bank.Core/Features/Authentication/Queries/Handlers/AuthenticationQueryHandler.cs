using Bank.Core.Bases;
using Bank.Core.Features.Authentication.Queries.Models;
using Bank.Services.Abstracts;
using MediatR;

namespace Bank.Core.Features.Authentication.Queries.Handlers
{
    public class AuthenticationQueryHandler : IRequestHandler<ConfirmEmailQuery, Response<string>>,
                                              IRequestHandler<ConfirmResetPasswordQuery, Response<string>>
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationQueryHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Response<string>> Handle(ConfirmEmailQuery request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.ConfirmEmail(request.Email, request.code);

            if (result == "ErrorWhenConfirmEmail")
            {
                return new Response<string>
                {
                    Success = false,
                    Message = "Email confirmation failed. Please try again."
                };
            }

            return new Response<string>
            {
                Success = true,
                Message = "Email confirmed successfully."
            };
        }

        public async Task<Response<string>> Handle(ConfirmResetPasswordQuery request, CancellationToken cancellationToken)
        {
            // Proceed with password reset confirmation logic
            var result = await _authenticationService.ConfirmResetPasswordAsync(request.Email, request.Code);

            if (result == "Failed")
            {
                return new Response<string>
                {
                    Success = false,
                    Message = "Password reset confirmation failed. Please ensure the code is valid."
                };
            }

            return new Response<string>
            {
                Success = true,
                Message = "Password reset successfully confirmed."
            };
        }
    }
}
