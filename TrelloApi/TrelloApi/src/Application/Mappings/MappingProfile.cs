using AutoMapper;
using TrelloApi.Domain.Board;
using TrelloApi.Domain.Comment;
using TrelloApi.Domain.Comment.DTO;
using TrelloApi.Domain.Entities.Board;
using TrelloApi.Domain.Entities.Comment;
using TrelloApi.Domain.Entities.List;
using TrelloApi.Domain.Entities.Task;
using TrelloApi.Domain.Entities.TaskLabel;
using TrelloApi.Domain.Entities.User;
using TrelloApi.Domain.Entities.UserTask;
using TrelloApi.Domain.Label;
using TrelloApi.Domain.Label.DTO;
using TrelloApi.Domain.Task.DTO;
using TrelloApi.Domain.TaskLabel.Dto;
using TrelloApi.Domain.User;
using TrelloApi.Domain.User.DTO;
using TrelloApi.Domain.UserBoard;
using TrelloApi.Domain.UserBoard.DTO;
using TrelloApi.Domain.UserTask;
using TrelloApi.Domain.UserTask.Dto;
using Task = TrelloApi.Domain.Task.Task;

namespace TrelloApi.Application.Utils;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Board, OutputBoardDto>();
        CreateMap<Comment, OutputCommentDto>();
        CreateMap<Label, OutputLabelDto>();
        CreateMap<List, OutputListDto>();
        CreateMap<Task, OutputTaskDto>();
        CreateMap<TaskLabel, OutputTaskLabelDto>();
        CreateMap<User, OutputUserDto>();
        CreateMap<UserBoard, OutputUserBoardDto>();
        CreateMap<UserTask, OutputUserTaskDto>();
    }
}