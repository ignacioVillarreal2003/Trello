using FluentValidation;
using TrelloApi.Domain.DTOs.Card;

namespace TrelloApi.Domain.DTOs.Comment;

public class AddCommentDtoValidator : AbstractValidator<AddCommentDto>
{
    public AddCommentDtoValidator()
    {
        RuleFor(comment => comment.Text)
            .NotEmpty().WithMessage("The text is required")
            .MaximumLength(256).WithMessage("The text should be of maximum 256 characters");
    }
}