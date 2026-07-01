using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices
{
    public interface ITrilhaService
    {
        Task<IEnumerable<TrilhaResponse>> ObterPorCampingAsync(long campingId, Guid? usuarioId = null);
        Task<TrilhaResponse?> ObterDetalheAsync(long trilhaId, Guid? usuarioId = null);
        Task<TrilhaResponse> CriarAsync(CriarTrilhaRequest request);
        Task<IEnumerable<TrilhaResponse>> ObterIndependentesParaMapaAsync();
    }
}
