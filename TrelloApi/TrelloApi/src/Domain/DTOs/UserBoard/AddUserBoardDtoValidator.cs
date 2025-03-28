using FluentValidation;

namespace TrelloApi.Domain.DTOs.UserBoard;

public class AddUserBoardDtoValidator : AbstractValidator<AddUserBoardDto>
{
    public AddUserBoardDtoValidator()
    {
        RuleFor(userBoard => userBoard.UserId)
            .NotEmpty().WithMessage("The user id is required");
    }
}