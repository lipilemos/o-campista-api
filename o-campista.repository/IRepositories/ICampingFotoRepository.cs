using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories
{
    public interface ICampingFotoRepository
    {
        Task<IEnumerable<CampingFoto>> ObterPorCampingAsync(long campingId);
    }
}
