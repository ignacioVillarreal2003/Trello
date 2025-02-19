using AutoMapper;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;

namespace TrelloApi.Application.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Board, OutputBoardDetailsDto>();
        CreateMap<Card, OutputCardDetailsDto>();
        CreateMap<CardLabel, OutputCardLabelDetailsDto>();
        CreateMap<Comment, OutputCommentDetailsDto>();
        CreateMap<Label, OutputLabelDetailsDto>();
        CreateMap<List, OutputListDetailsDto>();
        CreateMap<UserBoard, OutputUserBoardDetailsDto>();
        CreateMap<UserCard, OutputUserCardDetailsDto>();
        CreateMap<User, OutputUserDetailsDto>();
    }
}