using AutoMapper;
using Bank.Core.Bases;
using Bank.Core.Features.Authentication.Commands.Models;
using Bank.Core.Features.Authentication.Commands.Validatiors;
using Bank.Data.Entities.Identity;
using Bank.Data.Helpers;
using Bank.Services.Abstracts;
using MediatR;

namespace Bank.Core.Features.Authentication.Commands.Handlers
{
    public class AuthenticationCommandHandler : IRequestHandler<RegisterCommand, Response<AuthModel>>,
                                                IRequestHandler<RegisterWithRoleCommand, Response<AuthModel>>,
                                                IRequestHandler<LoginCommand, Response<AuthModel>>,
                                                IRequestHandler<ResetPasswordCommand, Response<string>>,
                                                IRequestHandler<SendResetPasswordCommand, Response<string>>

    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public AuthenticationCommandHandler(IAuthenticationService authenticationService, IMapper mapper)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        public async Task<Response<AuthModel>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var identityUser = _mapper.Map<ApplicationUser>(request);

            // Register the user using the AuthenticationService
            var createResult = await _authenticationService.RegisterAsync(identityUser, request.Password);

            // Check if registration was successful
            if (!string.IsNullOrEmpty(createResult.Message))
            {
                // If there is a message (like an error), return a failure response
                return new Response<AuthModel>
                {
                    Success = false,
                    Message = createResult.Message
                };
            }

            // Return a success response with the token or other relevant data
            return new Response<AuthModel>
            {
                Success = true,
                Message = "Registration successful",
                Data = new AuthModel
                {
                    Message = createResult.Message,
                    IsAuthenticated = createResult.IsAuthenticated,
                    Email = createResult.Email,
                    Username = createResult.Username,
                    Token = createResult.Token,
                    ExpiresOn = createResult.ExpiresOn,
                    Roles = createResult.Roles,
                    RefreshToken = createResult.RefreshToken,
                    RefreshTokenExpiration = createResult.RefreshTokenExpiration
                }
            };
        }

        public async Task<Response<AuthModel>> Handle(RegisterWithRoleCommand request, CancellationToken cancellationToken)
        {
            var identityUser = _mapper.Map<ApplicationUser>(request);

            // Register the user using the AuthenticationService
            var createResult = await _authenticationService.RegisterAsync(identityUser, request.RoleName, request.Password);

            // Check if registration was successful
            if (!string.IsNullOrEmpty(createResult.Message))
            {
                // If there is a message (like an error), return a failure response
                return new Response<AuthModel>
                {
                    Success = false,
                    Message = createResult.Message
                };
            }

            // Return a success response with the token or other relevant data
            return new Response<AuthModel>
            {
                Success = true,
                Message = "Registration successful",
                Data = new AuthModel
                {
                    Message = createResult.Message,
                    IsAuthenticated = createResult.IsAuthenticated,
                    Email = createResult.Email,
                    Username = createResult.Username,
                    Token = createResult.Token,
                    ExpiresOn = createResult.ExpiresOn,
                    Roles = createResult.Roles,
                    RefreshToken = createResult.RefreshToken,
                    RefreshTokenExpiration = createResult.RefreshTokenExpiration
                }
            };
        }

        public async Task<Response<AuthModel>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Proceed with the authentication logic
            var authResult = await _authenticationService.LoginAsync(request.UsernameOrEmail, request.Password);

            if (!authResult.IsAuthenticated)
            {
                return new Response<AuthModel>
                {
                    Success = false,
                    Message = authResult.Message ?? "Invalid credentials."
                };
            }

            return new Response<AuthModel>
            {
                Success = true,
                Message = "Login successful",
                Data = new AuthModel
                {
                    Message = authResult.Message,
                    IsAuthenticated = authResult.IsAuthenticated,
                    Email = authResult.Email,
                    Username = authResult.Username,
                    Token = authResult.Token,
                    ExpiresOn = authResult.ExpiresOn,
                    Roles = authResult.Roles,
                    RefreshToken = authResult.RefreshToken,
                    RefreshTokenExpiration = authResult.RefreshTokenExpiration
                }
            };
        }

        public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // Validate the request using FluentValidation
            var validator = new ResetPasswordValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                // Return a response with validation errors
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return new Response<string>
                {
                    Success = false,
                    Message = "Validation failed: " + errors
                };
            }

            // Proceed with password reset logic
            var result = await _authenticationService.ResetPasswordAsync(request.Email, request.Password);

            if (result == "Failed")
            {
                return new Response<string>
                {
                    Success = false,
                    Message = "Password reset failed. Please try again."
                };
            }

            return new Response<string>
            {
                Success = true,
                Message = "Password reset successful."
            };
        }

        public async Task<Response<string>> Handle(SendResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var validator = new SendResetPasswordCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return new Response<string>
                {
                    Success = false,
                    Message = "Validation failed: " + errors
                };
            }

            var result = await _authenticationService.SendResetPasswordAsync(request.Email);

            if (result == "Failed")
            {
                return new Response<string>
                {
                    Success = false,
                    Message = "Failed to send reset password email. Please try again later."
                };
            }

            return new Response<string>
            {
                Success = true,
                Message = "Reset password email sent successfully."
            };
        }
    }
}
