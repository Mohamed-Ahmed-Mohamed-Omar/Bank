using Bank.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace Bank.Core.Features.Authentication.Commands.Validatiors
{
    public class RegisterWithRoleValidator : AbstractValidator<RegisterWithRoleCommand>
    {
        public RegisterWithRoleValidator() 
        {
            ApplyValidationsRules();
        }

        public void ApplyValidationsRules()
        {
            // Validate that Username is not empty and meets length requirements
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");

            // Validate that Email is not empty and is in valid email format
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            // Validate that Password is not empty and meets strength requirements
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            // Validate that ConfirmPassword matches Password
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required.")
                .Equal(x => x.Password).WithMessage("Password and Confirm Password must match.");

            // Validate that CreditCard is not empty and has valid format
            RuleFor(x => x.CreditCardNumber)
                .NotEmpty().WithMessage("Credit Card number is required.")
                .CreditCard().WithMessage("Invalid credit card format.");

            // Validate RoleName - Ensure it's not empty and meets basic requirements
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role Name is required.")
                .Length(3, 50).WithMessage("Role Name must be between 3 and 50 characters.");

            // Optionally, you could add custom validation here for RoleName, e.g., ensure it's a valid role.
            RuleFor(x => x.RoleName)
                .MustAsync(async (roleName, cancellationToken) => await RoleExistsAsync(roleName))
                .WithMessage("Role name does not exist.");
        }

        // Custom method to check if the role exists (you can implement this as needed)
        private async Task<bool> RoleExistsAsync(string roleName)
        {
            // Implement logic to check if the role exists in your system.
            // For example, query the database or check a predefined list of roles.

            var validRoles = new List<string> { "Admin", "User", "Accountant" }; // Example valid roles
            return validRoles.Contains(roleName);
        }
    }
}
