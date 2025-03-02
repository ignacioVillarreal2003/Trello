using FluentValidation;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs.Board;

public class UpdateBoardDtoValidator : AbstractValidator<UpdateBoardDto>
{
    public UpdateBoardDtoValidator()
    {
        RuleFor(board => board.Title)
            .MaximumLength(32).WithMessage("The title should be of maximum 32 characters");

        RuleFor(board => board.Description)
            .MaximumLength(256).WithMessage("The description should be of maximum 256 characters");

        RuleFor(board => board.Background)
            .MaximumLength(32).WithMessage("The background should be of maximum 32 characters")
            .Must(background => background == null || BoardBackgroundValues.BoardBackgroundsAllowed.Contains(background))
            .WithMessage($"The background must bo one of: {string.Join(", ", BoardBackgroundValues.BoardBackgroundsAllowed)}");
    }
}