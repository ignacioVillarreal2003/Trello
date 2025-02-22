using FluentValidation;

namespace TrelloApi.Domain.DTOs.List;

public class AddListDtoValidator : AbstractValidator<AddListDto>
{
    public AddListDtoValidator()
    {
        RuleFor(list => list.Title)
            .NotEmpty().WithMessage("The title is required")
            .MaximumLength(32).WithMessage("The title should be of maximum 32 characters");

        RuleFor(list => list.Position)
            .NotEmpty().WithMessage("The position is required");
    }
}
