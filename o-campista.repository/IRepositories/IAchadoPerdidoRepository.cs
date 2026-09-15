using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories;

public interface IAchadoPerdidoRepository
{
    Task<AchadoPerdido> CriarAsync(AchadoPerdido item);
    Task<AchadoPerdido?> ObterPorIdAsync(long id);
    Task<List<AchadoPerdido>> ListarPorCampingAsync(
        long campingId, string? tipo, bool incluirResolvidos, int pagina, int limite);
    Task AtualizarAsync(AchadoPerdido item);
    Task DeletarAsync(AchadoPerdido item);

    /// <summary>
    /// True se existe um item aberto e recente publicado por um dos usuários num camping
    /// onde o outro já fez check-in. Libera a DM entre quem achou e quem perdeu.
    /// </summary>
    Task<bool> ExisteVinculoAtivoAsync(Guid usuarioA, Guid usuarioB);
}
