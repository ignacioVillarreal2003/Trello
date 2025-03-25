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
        CreateMap<Board, BoardResponse>();
        CreateMap<Card, CardResponse>();
        CreateMap<CardLabel, CardLabelResponse>()
            .ForMember(dest => dest.CardId, opt => opt.MapFrom(src => src.CardId))
            .ForMember(dest => dest.LabelId, opt => opt.MapFrom(src => src.LabelId))
            .ForMember(dest => dest.Card, opt => opt.MapFrom(src => src.Card))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label));
        CreateMap<Comment, CommentResponse>();
        CreateMap<Label, LabelResponse>();
        CreateMap<List, ListResponse>();
        CreateMap<UserBoard, UserBoardResponse>()
            .ForMember(dest => dest.BoardId, opt => opt.MapFrom(src => src.BoardId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Board, opt => opt.MapFrom(src => src.Board));
        CreateMap<UserCard, UserCardResponse>()
            .ForMember(dest => dest.CardId, opt => opt.MapFrom(src => src.CardId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Card, opt => opt.MapFrom(src => src.Card));
        CreateMap<User, UserResponse>();
    }
}