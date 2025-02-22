using FluentValidation;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs.Board;

public class AddBoardDtoValidator : AbstractValidator<AddBoardDto>
{
    public AddBoardDtoValidator()
    {
        RuleFor(board => board.Title)
            .NotEmpty().WithMessage("The title is required")
            .MaximumLength(32).WithMessage("The title should be of maximum 32 characters");

        RuleFor(board => board.Description)
            .MaximumLength(256).WithMessage("The description should be of maximum 256 characters");

        RuleFor(board => board.Background)
            .NotEmpty().WithMessage("The background is required")
            .MaximumLength(32).WithMessage("The background should be of maximum 32 characters")
            .Must(background => BoardBackgroundValues.BoardBackgroundsAllowed.Contains(background))
            .WithMessage($"The background must bo one of: {string.Join(", ", BoardBackgroundValues.BoardBackgroundsAllowed)}");;
    }
}