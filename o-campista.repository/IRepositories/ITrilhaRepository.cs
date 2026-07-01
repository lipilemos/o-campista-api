using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories
{
    public interface ITrilhaRepository
    {
        Task<int> ContarConcluidasPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<Trilha>> ObterPorCampingAsync(long campingId);
        Task<Trilha?> ObterPorIdComPontosAsync(long trilhaId);
        Task<Trilha> CriarAsync(Trilha trilha);
        Task<IEnumerable<Trilha>> ObterIndependentesAsync();
        Task AtualizarMediaAvaliacaoAsync(long trilhaId, double media);
    }
}
