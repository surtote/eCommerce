using FluentValidation;

namespace Identity.Models.Roles.Request
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
    {
        public CreateRoleRequestValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("RoleName is required.")
                .MinimumLength(3).WithMessage("RoleName must be at least 3 characters.");
        }
    }

}
