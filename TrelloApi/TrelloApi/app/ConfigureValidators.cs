using FluentValidation;
using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.DTOs.Comment;
using TrelloApi.Domain.DTOs.Label;
using TrelloApi.Domain.DTOs.List;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserBoard;
using TrelloApi.Domain.DTOs.UserCard;

namespace TrelloApi.app;

public static class ConfigureValidators
{
    public static IServiceCollection AddValidatorsFromAssemblies(this IServiceCollection validators)
    {
        validators.AddScoped<IValidator<AddBoardDto>, AddBoardDtoValidator>();
        validators.AddScoped<IValidator<UpdateBoardDto>, UpdateBoardDtoValidator>();
        validators.AddScoped<IValidator<AddCardDto>, AddCardDtoValidator>();
        validators.AddScoped<IValidator<UpdateCardDto>, UpdateCardDtoValidator>();
        validators.AddScoped<IValidator<AddCardLabelDto>, AddCardLabelDtoValidator>();
        validators.AddScoped<IValidator<AddCommentDto>, AddCommentDtoValidator>();
        validators.AddScoped<IValidator<UpdateCommentDto>, UpdateCommentDtoValidator>();
        validators.AddScoped<IValidator<AddLabelDto>, AddLabelDtoValidator>();
        validators.AddScoped<IValidator<UpdateLabelDto>, UpdateLabelDtoValidator>();
        validators.AddScoped<IValidator<AddListDto>, AddListDtoValidator>();
        validators.AddScoped<IValidator<UpdateListDto>, UpdateListDtoValidator>();
        validators.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
        validators.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
        validators.AddScoped<IValidator<UpdateUserDto>, UpdateUserDtoValidator>();
        validators.AddScoped<IValidator<AddUserBoardDto>, AddUserBoardDtoValidator>();
        validators.AddScoped<IValidator<AddUserCardDto>, AddUserCardDtoValidator>();
        return validators;
    }
}