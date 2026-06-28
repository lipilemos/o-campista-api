using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories
{
    public interface IUsuarioTrilhaRepository
    {
        Task<UsuarioTrilha?> ObterAsync(Guid usuarioId,long trilhaId);

        Task CriarAsync(UsuarioTrilha usuarioTrilha);

        Task AtualizarAsync(UsuarioTrilha usuarioTrilha);

        Task<int> ContarConcluidasPorUsuarioAsync(Guid usuarioId);
        }
    
}
