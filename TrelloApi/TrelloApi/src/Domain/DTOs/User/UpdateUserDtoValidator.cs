using FluentValidation;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs.User;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(label => label.Username)
            .MaximumLength(64).WithMessage("The username should be of maximum 64 characters");

        RuleFor(label => label.OldPassword)
            .MaximumLength(64).WithMessage("The old password should be of maximum 64 characters");
        
        RuleFor(label => label.NewPassword)
            .MaximumLength(64).WithMessage("The new password should be of maximum 64 characters");
        
        RuleFor(label => label.Theme)
            .MaximumLength(64).WithMessage("The theme should be of maximum 64 characters")
            .Must(theme => UserThemeValues.UserThemesAllowed.Contains(theme))
            .WithMessage($"The theme must bo one of: {string.Join(", ", UserThemeValues.UserThemesAllowed)}");
    }
}