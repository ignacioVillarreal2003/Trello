using FluentValidation;

namespace TrelloApi.Domain.DTOs.User;

public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("The email is required")
            .MaximumLength(64).WithMessage("The email should be of maximum 64 characters")
            .EmailAddress().WithMessage("The property email must be a valid email.");
        
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("The password is required")
            .MaximumLength(64).WithMessage("The password should be of maximum 64 characters");
    }
}