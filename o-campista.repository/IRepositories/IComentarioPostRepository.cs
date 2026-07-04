using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories;

public interface IComentarioPostRepository
{
    Task<ComentarioPost> CriarAsync(ComentarioPost comentario);
    Task<ComentarioPost?> ObterPorIdAsync(long id);
    Task<List<ComentarioPost>> ObterPorPostAsync(long postId, int pagina, int limite);
    Task DeletarAsync(long id);
    Task<int> ContarPorPostAsync(long postId);
}
