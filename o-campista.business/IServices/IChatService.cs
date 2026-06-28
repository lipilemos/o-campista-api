using o_campista.entities.Entities;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices
{
    public interface IChatService
    {
        Task<bool> TemCheckinValidoAsync(Guid usuarioId, long campingId);
        Task<MensagemChat> SalvarMensagemAsync(Guid usuarioId, long campingId, string texto);
        Task<List<MensagemChatResponse>> ObterHistoricoAsync(long campingId, int limite, DateTime? antes);
    }
}
