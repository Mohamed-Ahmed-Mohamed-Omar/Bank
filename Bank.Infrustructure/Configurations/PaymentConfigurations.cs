using Bank.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrustructure.Configurations
{
    public class PaymentConfigurations : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // Set primary key
            builder.HasKey(p => p.Id);

            // Configure relationships
            builder.HasOne(p => p.Account)
                   .WithMany() // Assuming no navigation property in Account for payments
                   .HasForeignKey(p => p.AccountId)
                   .OnDelete(DeleteBehavior.Restrict); // Avoid cascade deletion

            // Optional relationship for receiver's account
            builder.HasOne<Account>()
                   .WithMany() // Assuming no navigation property in Account for received payments
                   .HasForeignKey(p => p.ReceiverAccountId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
