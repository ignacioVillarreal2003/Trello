using FluentValidation;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs.Card;

public class UpdateCardDtoValidator: AbstractValidator<UpdateCardDto>
{
    public UpdateCardDtoValidator()
    {
        RuleFor(card => card.Title)
            .MaximumLength(32).WithMessage("The title should be of maximum 32 characters");

        RuleFor(card => card.Description)
            .MaximumLength(256).WithMessage("The description should be of maximum 256 characters");

        RuleFor(card => card.Priority)
            .MaximumLength(32).WithMessage("The priority should be of maximum 32 characters")
            .Must(priority => priority == null || PriorityValues.PrioritiesAllowed.Contains(priority))
            .WithMessage($"The priority must bo one of : {string.Join(", ", PriorityValues.PrioritiesAllowed)}");
    }
}