using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        // The account making the payment
        public int AccountId { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }

        // The recipient's account (if applicable)
        public int? ReceiverAccountId { get; set; } 

        // Amount to be paid
        public decimal Amount { get; set; }

        // e.g., "Transfer", "Bill Payment", "Loan Payment"
        public string PaymentType { get; set; }

        // Details or purpose of the payment
        public string Description { get; set; }

        // The date the payment was made
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // e.g., "Pending", "Completed", "Failed"
        public string Status { get; set; }

        // Optional reference number for tracking
        public string ReferenceNumber { get; set; }

        // e.g., "Credit Card", "Bank Transfer", "Cash
        public string PaymentMethod { get; set; } 
    }
}
