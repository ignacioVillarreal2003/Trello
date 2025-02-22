using FluentValidation;
using TrelloApi.Domain.DTOs.UserBoard;

namespace TrelloApi.Domain.DTOs.UserCard;

public class AddUserCardDtoValidator : AbstractValidator<AddUserCardDto>
{
    public AddUserCardDtoValidator()
    {
        RuleFor(userCard => userCard.UserId)
            .NotEmpty().WithMessage("The user id is required");
    }
}