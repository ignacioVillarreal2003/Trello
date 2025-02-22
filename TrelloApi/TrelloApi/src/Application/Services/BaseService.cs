using AutoMapper;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public abstract class BaseService
{
    protected readonly IMapper _mapper;
    protected readonly IUnitOfWork _unitOfWork;
    
    protected BaseService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
}