using AutoMapper;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;

namespace TrelloApi.Application.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Board, OutputBoardDetailsDto>();
        CreateMap<Board, OutputBoardListDto>();
        CreateMap<Card, OutputCardDetailsDto>();
        CreateMap<Card, OutputCardListDto>();
        CreateMap<CardLabel, OutputCardLabelListDto>();
        CreateMap<Comment, OutputCommentDetailsDto>();
        CreateMap<Label, OutputLabelDetailsDto>();

        
            
        CreateMap<List, OutputListDto>();
        CreateMap<User, OutputUserDto>();
        CreateMap<User, OutputUserBoardDetailsDto>();
        CreateMap<UserBoard, OutputUserBoardDto>();
        CreateMap<UserCard, OutputUserTaskDto>();
    }
}