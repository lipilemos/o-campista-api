using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface IFavoritoCampingService
{
    Task FavoritarAsync(Guid usuarioId, long campingId);
    Task DesfavoritarAsync(Guid usuarioId, long campingId);
    Task<List<CampingMapaResponse>> ObterFavoritosAsync(Guid usuarioId);
}
