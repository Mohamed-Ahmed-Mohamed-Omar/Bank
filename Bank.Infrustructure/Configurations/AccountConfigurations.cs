using Bank.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrustructure.Configurations
{
    public class AccountConfigurations : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // Set primary key
            builder.HasKey(a => a.Id);

            // Configure relationships
            builder.HasOne(a => a.User)
                   .WithMany(u => u.Accounts) // Assuming ApplicationUser has a collection of Accounts
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascade deletion

            // Configure navigation properties
            builder.HasMany(a => a.Payments)
                   .WithOne(p => p.Account)
                   .HasForeignKey(p => p.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
