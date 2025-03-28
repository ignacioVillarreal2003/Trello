using AutoMapper;
using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.DTOs.Comment;
using TrelloApi.Domain.DTOs.Label;
using TrelloApi.Domain.DTOs.List;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserBoard;
using TrelloApi.Domain.DTOs.UserCard;
using TrelloApi.Domain.Entities;

namespace TrelloApi.Application.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Board, BoardResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Background, opt => opt.MapFrom(src => src.Background))
            .ForMember(dest => dest.Lists, opt => opt.MapFrom(src => src.Lists))
            .ForMember(dest => dest.UserBoards, opt => opt.MapFrom(src => src.UserBoards))
            .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.Labels));
        CreateMap<Card, CardResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.ListId, opt => opt.MapFrom(src => src.ListId))
            .ForMember(dest => dest.CardLabels, opt => opt.MapFrom(src => src.CardLabels))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
            .ForMember(dest => dest.List, opt => opt.MapFrom(src => src.List));
        CreateMap<CardLabel, CardLabelResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CardId, opt => opt.MapFrom(src => src.CardId))
            .ForMember(dest => dest.LabelId, opt => opt.MapFrom(src => src.LabelId))
            .ForMember(dest => dest.Card, opt => opt.MapFrom(src => src.Card))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label));
        CreateMap<Comment, CommentResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
            .ForMember(dest => dest.CardId, opt => opt.MapFrom(src => src.CardId))
            .ForMember(dest => dest.Card, opt => opt.MapFrom(src => src.Card))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
        CreateMap<Label, LabelResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Board, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CardLabels, opt => opt.MapFrom(src => src.UpdatedAt));
        CreateMap<List, ListResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
            .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => src.BoardId))
            .ForMember(dest => dest.Board, opt => opt.MapFrom(src => src.Board))
            .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src.Cards));
        CreateMap<UserBoard, UserBoardResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => src.BoardId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Board, opt => opt.MapFrom(src => src.Board));
        CreateMap<UserCard, UserCardResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CardId, opt => opt.MapFrom(src => src.CardId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Card, opt => opt.MapFrom(src => src.Card));
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Theme, opt => opt.MapFrom(src => src.Theme))
            .ForMember(dest => dest.AvatarBackground, opt => opt.MapFrom(src => src.AvatarBackground))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
            .ForMember(dest => dest.UserBoards, opt => opt.MapFrom(src => src.UserBoards))
            .ForMember(dest => dest.UserCards, opt => opt.MapFrom(src => src.UserCards));
    }
}