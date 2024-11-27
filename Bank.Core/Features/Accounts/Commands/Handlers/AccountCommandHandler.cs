using AutoMapper;
using Bank.Core.Bases;
using Bank.Services.Abstracts;
using Bank.Data.Entities;
using MediatR;
using Bank.Core.Features.Accounts.Commands.Models;
using Bank.Services.AuthServices.Interfaces;

namespace Bank.Core.Features.Accounts.Commands.Handlers
{
    public class AccountCommandHandler : IRequestHandler<AddAccountCommand, Response<string>>,
                                         IRequestHandler<DeleteAccountCommand, Response<string>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccountServices _accountServices;
        private readonly IMapper _mapper;

        public AccountCommandHandler(IAccountServices accountServices, IMapper mapper, ICurrentUserService currentUserService)
        {
            _accountServices = accountServices;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Response<string>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the username from the current user context
            var username = _currentUserService.GetUserName();

            //mapping Between request and account
            var account = _mapper.Map<Account>(request);

            try
            {
                // Attempt to create the account
                var result = await _accountServices.CreateAccounAsync(account, username);

                if (result == "Success")
                {
                    return new Response<string>
                    {
                        Success = true,
                        Message = "Account created successfully."
                    };
                }

                // Handle specific service-layer errors
                return new Response<string>
                {
                    Success = false,
                    Message = result
                };
            }
            catch (Exception ex)
            {
                // Catch any unhandled exceptions and return a generic error message
                return new Response<string>
                {
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

        public async Task<Response<string>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountServices.DeleteAccountAsync(request.Id);

            if (result.Contains("not found"))
            {
                return new Response<string>
                {
                    Success = false,
                    Message = result
                };
            }

            return new Response<string>
            {
                Success = true,
                Message = result
            };
        }
    }
}
