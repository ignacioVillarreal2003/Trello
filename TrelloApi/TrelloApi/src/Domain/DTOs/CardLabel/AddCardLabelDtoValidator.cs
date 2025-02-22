using FluentValidation;

namespace TrelloApi.Domain.DTOs.CardLabel;

public class AddCardLabelDtoValidator : AbstractValidator<AddCardLabelDto>
{
    public AddCardLabelDtoValidator()
    {
        RuleFor(cardLabel => cardLabel.LabelId)
            .NotEmpty().WithMessage("The label id is required");
    }
}