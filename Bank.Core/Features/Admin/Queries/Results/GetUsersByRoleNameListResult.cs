namespace Bank.Core.Features.Admin.Queries.Results
{
    public class GetUsersByRoleNameListResult
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CreditCardNumber { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
