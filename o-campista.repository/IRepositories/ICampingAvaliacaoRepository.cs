using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories
{
    public interface ICampingAvaliacaoRepository
    {
        Task CriarAsync(CampingAvaliacao avaliacao);
        Task<CampingAvaliacao?> ObterPorIdAsync(long id);
        Task<CampingAvaliacao?> ObterPorCheckinAsync(long checkinId);
        Task<List<CampingAvaliacao>> ObterPorCampingAsync(long campingId);
        Task<List<CampingAvaliacao>> ObterPorCampingEUsuarioAsync(long campingId, Guid usuarioId, long? checkinId = null);
        Task<List<CampingAvaliacao>> ObterTodosAsync();
        Task AtualizarAsync(CampingAvaliacao avaliacao);
        Task DeletarAsync(long id);
    }
}
