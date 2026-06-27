using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices
{
    public interface ICampingAvaliacaoService
    {
        Task<CampingAvaliacaoResponse> CriarAvaliacaoAsync(CampingAvaliacaoRequest request);
        Task<CampingAvaliacaoResponse?> ObterPorIdAsync(long id);
        Task<List<CampingAvaliacaoComUsuarioResponse>> ObterAvaliacoesCampingAsync(long campingId);
        Task<List<CampingAvaliacaoComUsuarioResponse>> ObterAvaliacoesPorUsuarioAsync(long campingId, Guid usuarioId, long? checkinId = null);
        Task AtualizarAvaliacaoAsync(long id, CampingAvaliacaoRequest request);
        Task DeletarAvaliacaoAsync(long id);
    }
}
