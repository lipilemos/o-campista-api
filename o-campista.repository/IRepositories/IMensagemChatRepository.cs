using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories
{
    public interface IMensagemChatRepository
    {
        Task<MensagemChat> SalvarAsync(MensagemChat mensagem);
        Task<List<MensagemChat>> ObterHistoricoAsync(long campingId, int limite, DateTime? antes);
    }
}
