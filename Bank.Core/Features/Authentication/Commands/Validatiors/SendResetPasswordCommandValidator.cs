using Bank.Core.Features.Authentication.Commands.Models;
using FluentValidation;

namespace Bank.Core.Features.Authentication.Commands.Validatiors
{
    public class SendResetPasswordCommandValidator : AbstractValidator<SendResetPasswordCommand>
    {
        public SendResetPasswordCommandValidator() 
        {
             ApplyValidationsRules();
        }

        public void ApplyValidationsRules()
        {
            // Validate that Email is not empty
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
