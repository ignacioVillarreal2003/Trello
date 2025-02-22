using FluentValidation;

namespace TrelloApi.Domain.DTOs.User;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("The email is required")
            .MaximumLength(64).WithMessage("The email should be of maximum 64 characters")
            .EmailAddress().WithMessage("The property email must be a valid email.");
        
        RuleFor(user => user.Username)
            .NotEmpty().WithMessage("The username is required")
            .MaximumLength(64).WithMessage("The username should be of maximum 64 characters");
        
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("The password is required")
            .MaximumLength(64).WithMessage("The password should be of maximum 64 characters");
    }
}