using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories;

public interface IPostRepository
{
    Task<PostViagem> CriarAsync(PostViagem post);
    Task<PostViagem?> ObterPorIdAsync(long id);
    Task DeletarAsync(long id);
    Task CurtirAsync(CurtidaPost curtida);
    Task DescurtirAsync(long postId, Guid usuarioId);
    Task<bool> JaCurtiuAsync(long postId, Guid usuarioId);
    Task<int> ContarCurtidasAsync(long postId);
}
