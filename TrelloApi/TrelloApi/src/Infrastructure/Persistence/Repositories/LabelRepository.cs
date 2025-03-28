using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Generics;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Infrastructure.Persistence.Repositories;

public class LabelRepository: GenericRepository<Label>, ILabelRepository
{
    public LabelRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    
    public async Task<Label> GetLabelByIdToAccessAsync(int labelId)
    {
        return await Context.Labels
            .Include(l => l.Board)
            .FirstOrDefaultAsync(l => l.Id.Equals(labelId));
    }
}