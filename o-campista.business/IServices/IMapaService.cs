
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface IMapaService
{
    Task<IEnumerable<CampingMapaResponse>> ObterCampingsMapaAsync(string? busca = null, string? tipo = null, string[]? recursos = null);
}
