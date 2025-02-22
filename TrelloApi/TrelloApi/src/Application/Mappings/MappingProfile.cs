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
        CreateMap<CardLabel, CardLabelResponse>();
        CreateMap<Comment, CommentResponse>();
        CreateMap<Label, LabelResponse>();
        CreateMap<List, ListResponse>();
        CreateMap<UserBoard, UserBoardResponse>();
        CreateMap<UserCard, UserCardResponse>();
        CreateMap<User, UserResponse>();
    }
}