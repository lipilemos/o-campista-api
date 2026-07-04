using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface INotificacaoService
{
    Task CriarNotificacaoAsync(Guid destinatarioId, Guid remetenteId, string tipo, long? postId = null, string? postTexto = null, string? comentarioTexto = null);
    Task<List<NotificacaoResponse>> ObterNotificacoesAsync(Guid usuarioId, int pagina, int limite);
    Task<int> ContarNaoLidasAsync(Guid usuarioId);
    Task MarcarTodasComoLidasAsync(Guid usuarioId);
}
