using Bank.Core.Features.Admin.Commands.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Bank.Core.Features.Admin.Commands.Validators
{
    public class AddRoleValidators : AbstractValidator<AddRoleCommand>
    {
        private readonly RoleManager<IdentityRole> _roleService; // Inject the service to access the database

        public AddRoleValidators(RoleManager<IdentityRole> roleService)
        {
            ApplyValidationRules();
            _roleService = roleService;
        }

        private void ApplyValidationRules()
        {
            // Validate RoleName is not empty
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MustAsync(BeUniqueRoleName).WithMessage("Role name already exists in the database.");
        }

        // Method to check the uniqueness of the role name
        private async Task<bool> BeUniqueRoleName(string roleName, CancellationToken cancellationToken)
        {
            // Use the service to check if the role already exists
            return !await _roleService.RoleExistsAsync(roleName);
        }
    }
}
