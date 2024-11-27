using Bank.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace Bank.Core.Features.Authentication.Commands.Validatiors
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator() { }

        public void ApplyValidationsRules()
        {
            // Validate UsernameOrEmail (can be either a valid email or a non-empty username)
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage("Username or Email is required.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Invalid email format.")
                .When(x => !IsValidEmail(x.UsernameOrEmail), ApplyConditionTo.AllValidators)
                .WithMessage("Username is required when email is not provided.");

            // Validate Password (must be non-empty)
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }

        // Helper method to determine if the UsernameOrEmail is a valid email
        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
