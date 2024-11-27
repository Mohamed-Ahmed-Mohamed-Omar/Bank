using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;

namespace Bank.Data.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? CreditCardNumber { get; set; }

        [EncryptColumn]
        public string? Code { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
        public List<Account>? Accounts { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
