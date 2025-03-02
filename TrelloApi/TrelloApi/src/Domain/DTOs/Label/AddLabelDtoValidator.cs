using FluentValidation;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs.Label;

public class AddLabelDtoValidator : AbstractValidator<AddLabelDto>
{
    public AddLabelDtoValidator()
    {
        RuleFor(label => label.Title)
            .NotEmpty().WithMessage("The title is required")
            .MaximumLength(32).WithMessage("The title should be of maximum 32 characters");
        
        RuleFor(label => label.Color)
            .MaximumLength(8).WithMessage("The color should be of maximum 8 characters")
            .Must(color => LabelColorValues.LabelColorsAllowed.Contains(color))
            .WithMessage($"The color must bo one of: {string.Join(", ", LabelColorValues.LabelColorsAllowed)}");
    }
}
