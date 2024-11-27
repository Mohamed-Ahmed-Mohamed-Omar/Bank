using Bank.Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Data.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string AttachmentUrl { get; set; }
        public string SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public ApplicationUser Sender { get; set; }
        public string RecipientId { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public ApplicationUser Recipient { get; set; }
    }
}
