using Bank.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace Bank.Core.Features.Authentication.Commands.Validatiors
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator() 
        {
            ApplyValidationsRules();
        }

        public void ApplyValidationsRules()
        {
            // Username validation
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(5).WithMessage("Username must be at least 5 characters long.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            // Password validation
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            // Confirm Password validation
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            // Credit Card validation
            RuleFor(x => x.CreditCardNumber)
                .NotEmpty().WithMessage("Credit Card is required.")
                .CreditCard().WithMessage("Invalid Credit Card format.");

            // Phone Number validation
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9]{10,15}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Invalid phone number format.");
        }

    }
}
