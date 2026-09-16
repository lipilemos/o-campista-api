using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface ICampingParceiroService
{
    Task<CampingParceiroResponse> CriarAsync(Guid usuarioId, CampingParceiroRequest request);
    Task<CampingParceiroResponse> ReivindicarAsync(Guid usuarioId, long campingId);
    Task<List<CampingParceiroResponse>> ObterMeusAsync(Guid usuarioId);
    Task<List<CampingProximoResponse>> ObterProximosSemDonoAsync(decimal latitude, decimal longitude);
    Task<CampingPainelResponse> ObterPainelAsync(Guid usuarioId, long campingId);
}
