using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;

public class RecursoRepository : IRecursoRepository
{
    private readonly CampistaDbContext _context;

    public RecursoRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public Task<List<Recurso>> ObterTodosAsync()
    {
        return _context.Recursos.AsNoTracking().OrderBy(r => r.Nome).ToListAsync();
    }
}
