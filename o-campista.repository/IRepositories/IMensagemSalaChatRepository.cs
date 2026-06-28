using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories
{
    public interface IMensagemSalaChatRepository
    {
        Task<MensagemSalaChat> SalvarAsync(MensagemSalaChat mensagem);
        Task<List<MensagemSalaChat>> ObterHistoricoAsync(long salaId, int skip, int take);
        Task<Dictionary<long, int>> ContarNaoLidasAsync(Guid usuarioId);
        Task AtualizarUltimaMensagemLidaAsync(long salaId, Guid usuarioId, long ultimaMensagemId);
        Task<long?> ObterUltimaMensagemIdAsync(long salaId);
    }
}
