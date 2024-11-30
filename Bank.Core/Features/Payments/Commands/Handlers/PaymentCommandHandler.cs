using AutoMapper;
using Bank.Core.Bases;
using Bank.Core.Features.Payments.Commands.Models;
using Bank.Data.Entities;
using Bank.Services.Abstracts;
using Bank.Services.AuthServices.Interfaces;
using MediatR;

namespace Bank.Core.Features.Payments.Commands.Handlers
{
    public class PaymentCommandHandler : IRequestHandler<PaymentCommand, Response<string>>
    {
        private readonly IPaymentServices _paymentServices;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailsService _emailsService;
        private readonly IMapper _mapper;

        public PaymentCommandHandler(IPaymentServices paymentServices, ICurrentUserService currentUserService, IMapper mapper, IEmailsService emailsService)
        {
            _paymentServices = paymentServices;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _emailsService = emailsService;
        }

        public async Task<Response<string>> Handle(PaymentCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the current user's username from the service
            var username = _currentUserService.GetUserNameAsync();

            // Map the PaymentCommand to a Payment entity using AutoMapper
            var payment = _mapper.Map<Payment>(request);

            try
            {
                // Call the Payment service to process the payment
                var paymentResult = await _paymentServices.PaymentAsync(payment, username);

                // Get the email for the created user
                var email = await _currentUserService.GetEmailByUsernameAsync(username);

                if (paymentResult != null)
                {
                    // Map the status code to a string
                    string paymentStatus = paymentResult.Status == 1 ? "Completed" : "Failed";

                    var emailMessage = $@"
                        <html>
                            <body>
                                <p>Your payment has been processed successfully. Here are your payment details:</p>
                                <p><strong>Payment Reference Number:</strong> {paymentResult.ReferenceNumber}</p>
                                <p><strong>Paid:</strong> {paymentResult.Amount}</p>
                                <p><strong>Payment Type:</strong> {paymentResult.PaymentType}</p>
                                <p><strong>Description:</strong> {paymentResult.Description}</p>
                                <p><strong>Payment Date:</strong> {paymentResult.PaymentDate}</p>
                                <p><strong>Status:</strong> {paymentStatus}</p>
                                <p><strong>Balance:</strong> {paymentResult.Account.Balance}</p>
                                <p><strong>Payment Method:</strong> {paymentResult.PaymentMethod}</p>
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

                // Handle case where paymentResult is null
                return new Response<string>
                {
                    Success = false,
                    Message = "Payment processing failed."
                };
            }
            catch (Exception ex)
            {
                // Return error response if an exception occurs during payment processing
                return new Response<string>
                {
                    Success = false,
                    Message = $"An error occurred while processing the payment: {ex.Message}"
                };
            }
        }
    }
}
