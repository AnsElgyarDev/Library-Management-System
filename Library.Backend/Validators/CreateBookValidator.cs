using FluentValidation; 
using Practice.DTO;

namespace Library.Core.Validators;

public class CreateBookValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Author).
        NotEmpty().WithMessage("Required Field!")
        .MaximumLength(100).WithMessage("Too Much Length for a Name");

        RuleFor(x => x.Name).
        NotEmpty().WithMessage("Required Field!");

        RuleFor(x => x.Price).
        GreaterThan(-1).WithMessage("The Value Must Be More Than Or Equal Zero!");
    }
}