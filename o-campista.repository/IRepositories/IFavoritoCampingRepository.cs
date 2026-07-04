using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories;

public interface IFavoritoCampingRepository
{
    Task AdicionarAsync(UsuarioCampingFavorito favorito);
    Task RemoverAsync(Guid usuarioId, long campingId);
    Task<bool> ExisteAsync(Guid usuarioId, long campingId);
    Task<List<Camping>> ObterPorUsuarioAsync(Guid usuarioId);
}
