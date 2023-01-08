using FluentValidation;
using Friendbook.Shared.Models.Requests;

namespace Friendbook.Validations;

public class SavePersonRequestValidator : AbstractValidator<SavePersonRequest>
{
    public SavePersonRequestValidator()
    {
        RuleFor(p => p.FirstName).NotEmpty().MaximumLength(30);
        RuleFor(p => p.LastName).NotEmpty().MaximumLength(30);
        RuleFor(p => p.City).MaximumLength(50);
    }
}
