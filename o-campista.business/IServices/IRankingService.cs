using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface IRankingService
{
    Task<List<RankingItemResponse>> ObterRankingGlobalAsync(Guid usuarioId, int pagina, int limite);
    Task<List<RankingItemResponse>> ObterRankingSeguidosAsync(Guid usuarioId);
    Task<List<CampingRankingResponse>> ObterRankingCampingsAsync(int pagina, int limite);
}
