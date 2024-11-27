using Bank.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrustructure.Configurations
{
    public class MessageConfigurations : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            // Set primary key
            builder.HasKey(m => m.Id);

            // Configure relationships for Sender
            builder.HasOne(m => m.Sender)
                   .WithMany() // No navigation collection in ApplicationUser for sent messages
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascade deletion

            // Configure relationships for Recipient
            builder.HasOne(m => m.Recipient)
                   .WithMany() // No navigation collection in ApplicationUser for received messages
                   .HasForeignKey(m => m.RecipientId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascade deletion
        }
    }
}
