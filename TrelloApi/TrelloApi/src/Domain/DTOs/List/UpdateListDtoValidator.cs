using FluentValidation;

namespace TrelloApi.Domain.DTOs.List;

public class UpdateListDtoValidator : AbstractValidator<UpdateListDto>
{
    public UpdateListDtoValidator()
    {
        RuleFor(list => list.Title)
            .MaximumLength(32).WithMessage("The title should be of maximum 32 characters");
    }
}