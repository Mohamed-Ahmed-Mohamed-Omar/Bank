using Bank.Data.Entities;
using Bank.Data.Entities.Identity;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace Bank.Infrustructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IEncryptionProvider _encryptionProvider;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _encryptionProvider = new GenerateEncryptionProvider("8a4dcaaec64d412380fe4b02193cd26f");
        }

        public DbSet<Account> accounts { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<Message> messages { get; set; }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "a1d9b7f2-5c9e-4a7d-b12a-c32b8e08e16f", // GUID ثابت للدور Admin
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole
                {
                    Id = "b2f3c21a-345d-4570-97f7-e5e0d5eafc91", // GUID ثابت للدور User
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "2"
                },
                new IdentityRole
                {
                    Id = "c3f8a452-d78b-42e6-9b9f-987fb6a5f0c4", // GUID ثابت للدور Accountant
                    Name = "Accountant",
                    NormalizedName = "ACCOUNTANT",
                    ConcurrencyStamp = "3"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            SeedRoles(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Code)
                .HasConversion(
                    v => _encryptionProvider.Encrypt(v),  // التشفير
                    v => _encryptionProvider.Decrypt(v)   // فك التشفير
                );
        }
    }
}
