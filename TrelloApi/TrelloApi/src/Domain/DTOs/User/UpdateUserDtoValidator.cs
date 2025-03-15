using FluentValidation;
using TrelloApi.Domain.Constants;

namespace TrelloApi.Domain.DTOs.User;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(user => user.Username)
            .MaximumLength(64).WithMessage("The username should be of maximum 64 characters");

        RuleFor(user => user.OldPassword)
            .MaximumLength(64).WithMessage("The old password should be of maximum 64 characters");
        
        RuleFor(user => user.NewPassword)
            .MaximumLength(64).WithMessage("The new password should be of maximum 64 characters");
        
        RuleFor(user => user.Theme)
            .MaximumLength(64).WithMessage("The theme should be of maximum 64 characters")
            .Must(theme => theme == null || UserThemeValues.UserThemesAllowed.Contains(theme))
            .WithMessage($"The theme must bo one of: {string.Join(", ", UserThemeValues.UserThemesAllowed)}");
        
        RuleFor(user => user.AvatarBackground)
            .MaximumLength(32).WithMessage("The avatar background should be of maximum 32 characters")
            .Must(background => background == null || AvatarBackgroundValues.AvatarBackgroundsAllowed.Contains(background))
            .WithMessage($"The avatar background must bo one of: {string.Join(", ", AvatarBackgroundValues.AvatarBackgroundsAllowed)}");
    }
}