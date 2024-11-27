using Bank.Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Data.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
