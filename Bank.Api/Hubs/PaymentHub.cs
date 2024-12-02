using Bank.Data.Entities;
using Bank.Services.Abstracts;
using Microsoft.AspNetCore.SignalR;

namespace Bank.Api.Hubs
{
    public class PaymentHub : Hub
    {
        private readonly IPaymentServices _paymentServices;

        public PaymentHub(IPaymentServices paymentServices)
        {
            _paymentServices = paymentServices;
        }

        public async Task NotifyPaymentUpdate(string username, string PaymentMethod, string Description, string PaymentType, decimal Amount)
        {
            // Fetch payment details by paymentId using the payment service
            var paymentDetails = await _paymentServices.GetPaymentDetailsAsync(username, PaymentMethod, Description, PaymentType, Amount);

            if (paymentDetails != null)
            {

                // Get the Status as int and determine the description
                byte paymentStatus = paymentDetails.GetType().GetProperty("Status")?.GetValue(paymentDetails) as byte? ?? 0;
                string statusDescription = paymentStatus == 1 ? "Completed" : "Failed"; // Convert status to string description

                // Prepare the data to send to clients
                var paymentData = new
                {
                    Id = paymentDetails.GetType().GetProperty("Id")?.GetValue(paymentDetails) as int? ?? 0,
                    Username = username,
                    Amount = Amount,
                    Description = Description,
                    PaymentDate = paymentDetails.GetType().GetProperty("PaymentDate")?.GetValue(paymentDetails) as DateTime? ?? DateTime.MinValue,
                    PaymentMethod = PaymentMethod,
                    PaymentType = PaymentType,
                    ReferenceNumber = paymentDetails.GetType().GetProperty("ReferenceNumber")?.GetValue(paymentDetails) as int? ?? 0,
                    Status = statusDescription,
                    Email = paymentDetails.GetType().GetProperty("Email")?.GetValue(paymentDetails) as string
                };

                // Notify all connected clients with the updated payment information
                await Clients.All.SendAsync("ReceivePaymentUpdate", paymentData);
            }
        }
    }
}
