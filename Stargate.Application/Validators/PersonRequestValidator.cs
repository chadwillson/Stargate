using FluentValidation;
using Stargate.Domain.Dtos;

namespace Stargate.Application.Validators
{
    public class PersonRequestValidator : AbstractValidator<PersonRequest>
    {
        public PersonRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters");
        }
    }
}
