using FluentValidation;

namespace TrelloApi.Domain.DTOs.Comment;

public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentDtoValidator()
    {
        RuleFor(comment => comment.Text)
            .NotEmpty().WithMessage("The text is required")
            .MaximumLength(256).WithMessage("The text should be of maximum 256 characters");
    }
}