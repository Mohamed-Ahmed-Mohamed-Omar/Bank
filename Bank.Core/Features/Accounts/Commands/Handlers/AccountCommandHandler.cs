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
        private readonly IEmailsService _emailsService;
        private readonly IMapper _mapper;

        public AccountCommandHandler(IAccountServices accountServices, IMapper mapper, ICurrentUserService currentUserService, IEmailsService emailsService)
        {
            _accountServices = accountServices;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _emailsService = emailsService;
        }

        public async Task<Response<string>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the username from the current user context
            var username =  _currentUserService.GetUserNameAsync(); // تأكد من استخدام await هنا

            // Check if the user already has an account
            var existingAccount = await _accountServices.GetAccountByUsernameAsync(username);
            if (existingAccount != null)
            {
                return new Response<string>
                {
                    Success = false,
                    Message = "You already have an account. Please view your account details.",
                    RedirectUrl = "/Account/GetAccountByName" // Provide the URL to redirect
                };
            }

            // Mapping Between request and account
            var account = _mapper.Map<Account>(request);

            try
            {
                // Attempt to create the account
                var newAccount = await _accountServices.CreateAccounAsync(account, username);

                // Get the email for the created user
                var email = await _currentUserService.GetEmailByUsernameAsync(username);

                if (newAccount != null)
                {
                    // Send email after account creation
                    var emailMessage = $@"
                        <html>
                        <body>
                            <p>Your account has been created successfully. Here are your details:</p>
                            <table border='1'>
                                <tr>
                                    <td><strong>Account Number:</strong></td>
                                    <td>{newAccount.AccountNumber}</td>
                                </tr>
                                <tr>
                                    <td><strong>Balance:</strong></td>
                                    <td>{newAccount.Balance}</td>
                                </tr>
                                <tr>
                                    <td><strong>Created At:</strong></td>
                                    <td>{newAccount.CreatedAt}</td>
                                </tr>
                            </table>
                        </body>
                        </html>
                    ";
                    // Send the email and check the response (optional: check the actual result)
                    var emailResponse = await _emailsService.SendEmail(email, emailMessage, "Account Created");

                    // Return success response
                    return new Response<string>
                    {
                        Success = true,
                        Message = "Account created successfully and email sent."
                    };
                }

                // Handle case where account creation fails
                return new Response<string>
                {
                    Success = false,
                    Message = "Account creation failed."
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
