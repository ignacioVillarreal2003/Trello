using AutoMapper;
using TrelloApi.Application.Services.Interfaces;

namespace TrelloApi.Application.Services;

public abstract class BaseService
{
    protected readonly IMapper _mapper;
    protected readonly IBoardAuthorizationService _boardAuthorizationService;
    
    protected BaseService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService)
    {
        _mapper = mapper;
        _boardAuthorizationService = boardAuthorizationService;
    }
}