using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories;

public interface IRecursoRepository
{
    Task<List<Recurso>> ObterTodosAsync();
}
