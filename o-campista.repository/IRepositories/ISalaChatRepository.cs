using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories
{
    public interface ISalaChatRepository
    {
        Task<List<SalaChat>> ObterSalasDoUsuarioAsync(Guid usuarioId);
        Task<SalaChat?> ObterPorIdAsync(long salaId);
        Task<SalaChat> CriarAsync(SalaChat sala);
        Task<SalaChat?> ObterPorCampingIdAsync(long campingId);
        Task<SalaChat?> ObterPorCodigoConviteAsync(string codigo);
        Task AdicionarMembroAsync(SalaChatMembro membro);
        Task<List<SalaChatMembro>> ObterMembrosAsync(long salaId);
        Task<bool> UsuarioEhMembroAsync(long salaId, Guid usuarioId);
    }
}
