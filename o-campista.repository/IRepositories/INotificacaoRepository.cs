using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories;

public interface INotificacaoRepository
{
    Task CriarAsync(Notificacao notificacao);
    Task<List<Notificacao>> ObterPorDestinatarioAsync(Guid destinatarioId, int pagina, int limite);
    Task<int> ContarNaoLidasAsync(Guid destinatarioId);
    Task MarcarTodasComoLidasAsync(Guid destinatarioId);
}
