
using o_campista.api.Models.Responses;

namespace o_campista.api.Services
{
    public interface IMapaService
    {
        Task<IEnumerable<CampingMapaResponse>> ObterCampingsMapaAsync();
    }
}
