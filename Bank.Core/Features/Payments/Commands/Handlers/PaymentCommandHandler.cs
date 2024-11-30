using AutoMapper;
using Bank.Core.Bases;
using Bank.Core.Features.Payments.Commands.Models;
using Bank.Data.Entities;
using Bank.Services.Abstracts;
using Bank.Services.AuthServices.Interfaces;
using MediatR;

namespace Bank.Core.Features.Payments.Commands.Handlers
{
    public class PaymentCommandHandler : IRequestHandler<PaymentCommand, Response<string>>,
                                         IRequestHandler<TransferCommand, Response<string>>
    {
        private readonly IPaymentServices _paymentServices;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccountServices _accountServices;
        private readonly IEmailsService _emailsService;
        private readonly IMapper _mapper;

        public PaymentCommandHandler(IPaymentServices paymentServices, ICurrentUserService currentUserService, IMapper mapper, IEmailsService emailsService, IAccountServices accountServices)
        {
            _paymentServices = paymentServices;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _emailsService = emailsService;
            _accountServices = accountServices;
        }

        public async Task<Response<string>> Handle(PaymentCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the current user's username asynchronously
            var username = _currentUserService.GetUserNameAsync();

            // Map the PaymentCommand to a Payment entity using AutoMapper
            var payment = _mapper.Map<Payment>(request);

            // Get the email for the user
            var email = await _currentUserService.GetEmailByUsernameAsync(username);

            try
            {
                // Call the Payment service to process the payment
                var paymentResult = await _paymentServices.PaymentAsync(payment, username);

                if (paymentResult != null)
                {
                    // Prepare the success email content
                    var emailMessage = GeneratePaymentEmail(paymentResult, isSuccess: true);

                    // Send the email
                    await _emailsService.SendEmail(email, emailMessage, "Payment Processed Successfully");

                    // Return success response
                    return new Response<string>
                    {
                        Success = true,
                        Message = "Payment processed successfully, and email sent."
                    };
                }

                // Handle the case where paymentResult is null
                return new Response<string>
                {
                    Success = false,
                    Message = "Payment processing failed."
                };
            }
            catch (Exception ex)
            {
                // Prepare the failure email content
                var emailMessage = GeneratePaymentEmail(payment, isSuccess: false, errorMessage: ex.Message);

                // Send the failure email
                await _emailsService.SendEmail(email, emailMessage, "Payment Processing Failed");

                // Return error response
                return new Response<string>
                {
                    Success = false,
                    Message = $"An error occurred while processing the payment: {ex.Message}"
                };
            }
        }

        public async Task<Response<string>> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the sender's account using the username
                var senderUsername = _currentUserService.GetUserNameAsync();

                var senderAccount = await _accountServices.GetAccountByUsernameAsync(senderUsername);
                if (senderAccount == null)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "Sender's account not found."
                    };
                }

                // Retrieve the receiver's account using the account number
                var receiverAccount = await _accountServices.GetAccountByAccountNumberAsync(request.ReceiverAccountNumber);
                if (receiverAccount == null)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "Receiver's account not found."
                    };
                }

                // Validate the transfer amount
                if (request.Amount <= 0)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "Amount must be greater than zero."
                    };
                }

                // Check if the sender has sufficient balance
                if (senderAccount.Balance < request.Amount)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "Insufficient balance for transfer."
                    };
                }

                // Create the payment transfer
                var payment = new Payment
                {
                    AccountId = senderAccount.Id,
                    ReceiverAccountId = receiverAccount.Id,
                    Amount = request.Amount,
                    PaymentType = "Transfer",
                    Description = request.Description,
                    PaymentMethod = request.PaymentMethod,
                    PaymentDate = DateTime.UtcNow,
                    Status = 1, // Completed
                    ReferenceNumber = new Random().Next(100000, 999999) // Generate a random reference number
                };

                // Perform the transfer using the payment service
                var paymentResult = await _paymentServices.TransferAsync(payment, senderUsername);

                // Send email to sender
                var senderEmail = await _currentUserService.GetEmailByUsernameAsync(senderUsername);
                var senderEmailMessage = $@"
                <html>
                    <body>
                        <p>Your payment of {paymentResult.Amount} has been successfully processed.</p>
                        <p><strong>Payment Reference Number:</strong> {paymentResult.ReferenceNumber}</p>
                        <p><strong>Receiver's Account:</strong> {receiverAccount.AccountNumber}</p>
                        <p><strong>Payment Type:</strong> {paymentResult.PaymentType}</p>
                        <p><strong>Description:</strong> {paymentResult.Description}</p>
                        <p><strong>Payment Date:</strong> {paymentResult.PaymentDate}</p>
                        <p><strong>Balance:</strong> {payment.Account.Balance}</p>
                        <p><strong>Status:</strong> Completed</p>
                    </body>
                </html>
            ";
                await _emailsService.SendEmail(senderEmail, senderEmailMessage, "Transfer Completed");

                // Send email to receiver
                var receiverEmail = await _currentUserService.GetEmailByUsernameAsync(receiverAccount.UserName);// Assuming receiver's email is stored in UserName field
                var receiverEmailMessage = $@"
                <html>
                    <body>
                        <p>You have received a payment of {paymentResult.Amount}.</p>
                        <p><strong>Payment Reference Number:</strong> {paymentResult.ReferenceNumber}</p>
                        <p><strong>Sender's Account:</strong> {senderAccount.AccountNumber}</p>
                        <p><strong>Payment Type:</strong> {paymentResult.PaymentType}</p>
                        <p><strong>Description:</strong> {paymentResult.Description}</p>
                        <p><strong>Payment Date:</strong> {paymentResult.PaymentDate}</p>
                        <p><strong>Balance:</strong> {receiverAccount.Balance}</p>
                        <p><strong>Status:</strong> Completed</p>
                    </body>
                </html>
            ";
                await _emailsService.SendEmail(receiverEmail, receiverEmailMessage, "You’ve Received a Payment");

                return new Response<string>
                {
                    Success = true,
                    Message = "Transfer completed successfully and emails sent."
                };
            }
            catch (Exception ex)
            {
                return new Response<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        private string GeneratePaymentEmail(Payment payment, bool isSuccess, string errorMessage = null)
        {
            var status = isSuccess ? "Completed" : "Failed";
            var additionalMessage = !isSuccess && !string.IsNullOrEmpty(errorMessage)
                ? $"<p><strong>Error Message:</strong> {errorMessage}</p>"
                : string.Empty;

            return $@"
        <html>
            <body>
                <p>Your payment has been processed. Here are your payment details:</p>
                <p><strong>Payment Reference Number:</strong> {payment.ReferenceNumber}</p>
                <p><strong>Paid:</strong> {payment.Amount}</p>
                <p><strong>Payment Type:</strong> {payment.PaymentType}</p>
                <p><strong>Description:</strong> {payment.Description}</p>
                <p><strong>Payment Date:</strong> {payment.PaymentDate}</p>
                <p><strong>Status:</strong> {status}</p>
                <p><strong>Balance:</strong> {payment.Account.Balance}</p>
                <p><strong>Payment Method:</strong> {payment.PaymentMethod}</p>
                {additionalMessage}
            </body>
        </html>
    ";
        }
    }
}
