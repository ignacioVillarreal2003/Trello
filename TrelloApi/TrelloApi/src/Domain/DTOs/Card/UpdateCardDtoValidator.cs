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
        
        RuleFor(card => card.Position)
            .GreaterThanOrEqualTo(0).WithMessage("The position must be 0 or greater");
        
        RuleFor(card => card.ListId)
            .GreaterThanOrEqualTo(0).WithMessage("The list id must be 0 or greater");
    }
}