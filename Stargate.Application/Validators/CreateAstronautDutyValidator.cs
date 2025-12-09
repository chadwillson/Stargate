using FluentValidation;
using Stargate.Domain.Dtos;

namespace Stargate.Application.Validators
{
    public class CreateAstronautDutyValidator : AbstractValidator<CreateAstronautDutyResponse>
    {
        public CreateAstronautDutyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters");

            RuleFor(x => x.Rank)
                .NotEmpty().WithMessage("Rank is required")
                .MaximumLength(100).WithMessage("Rank cannot exceed 100 characters");

            RuleFor(x => x.DutyTitle)
                .NotEmpty().WithMessage("Duty Title is required")
                .MaximumLength(255).WithMessage("Duty Title cannot exceed 255 characters");

            RuleFor(x => x.DutyStartDate)
                .NotEmpty().WithMessage("Duty Start Date is required");
        }
    }
}
