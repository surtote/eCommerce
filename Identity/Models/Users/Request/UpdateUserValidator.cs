using FluentValidation;
using Identity.DTO;
using Identity.Models.Users.Request;


namespace Identity.Models.Users.Request
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El usuario es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("Formato de email inválido.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.Dni)
                .NotEmpty().WithMessage("El DNI es obligatorio.")
                .MaximumLength(20);

            RuleFor(x => x.Telefono)
                .GreaterThanOrEqualTo(0).When(x => x.Telefono.HasValue)
                .WithMessage("El teléfono debe ser un número positivo.");

            RuleFor(x => x.Direccion)
                .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Direccion));
        }
    }
}
