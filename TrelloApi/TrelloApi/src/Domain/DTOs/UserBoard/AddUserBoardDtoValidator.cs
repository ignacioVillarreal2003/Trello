using FluentValidation;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs.UserBoard;

public class AddUserBoardDtoValidator : AbstractValidator<AddUserBoardDto>
{
    public AddUserBoardDtoValidator()
    {
        RuleFor(userBoard => userBoard.UserId)
            .NotEmpty().WithMessage("The user id is required");
        
        RuleFor(userBoard => userBoard.Role)
            .NotEmpty().WithMessage("The role is required")
            .MaximumLength(32).WithMessage("The role should be of maximum 32 characters")
            .Must(role => RoleValues.RolesAllowed.Contains(role))
            .WithMessage($"The role must bo one of: {string.Join(", ", RoleValues.RolesAllowed)}");
    }
}