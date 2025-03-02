using FluentValidation;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs.Label;

public class UpdateLabelDtoValidator : AbstractValidator<UpdateLabelDto>
{
    public UpdateLabelDtoValidator()
    {
        RuleFor(label => label.Title)
            .MaximumLength(32).WithMessage("The title should be of maximum 32 characters");
        
        RuleFor(label => label.Color)
            .MaximumLength(8).WithMessage("The color should be of maximum 8 characters")
            .Must(color => color == null || LabelColorValues.LabelColorsAllowed.Contains(color))
            .WithMessage($"The color must bo one of: {string.Join(", ", LabelColorValues.LabelColorsAllowed)}");
    }
}