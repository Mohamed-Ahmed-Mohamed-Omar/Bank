using Bank.Core.Features.Accounts.Commands.Models;
using FluentValidation;

namespace Bank.Core.Features.Accounts.Commands.Validators
{
    public class AddAccountValidators : AbstractValidator<AddAccountCommand>
    {
        public AddAccountValidators() 
        {
            ApplyValidationRules();
        }

        private void ApplyValidationRules()
        {
           RuleFor(x => x.Balance)
            .LessThanOrEqualTo(0)
            .WithMessage("Balance cannot be negative.")
            .GreaterThanOrEqualTo(100)
            .WithMessage("Balance cannot be Less than 100.");
        }
    }
}
