
using LabMS.Contracts.Role;

namespace LabMS.Validators;

public class RoleCreateRequestValidator : AbstractValidator<RoleCreateRequest>
{
    public RoleCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required")
            .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Role name can only contain letters, numbers, spaces, hyphens, and underscores")
            .Must(name => name.ToLower() is not "admin" or "receptionist" or "technician" or "labmanager")
            .WithMessage("Cannot create system roles. Use: Admin, Receptionist, Technician, LabManager");
    }
}
