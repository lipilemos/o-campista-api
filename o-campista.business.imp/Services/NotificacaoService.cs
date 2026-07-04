using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class NotificacaoService : INotificacaoService
{
    private readonly INotificacaoRepository _notificacaoRepository;

    public NotificacaoService(INotificacaoRepository notificacaoRepository)
    {
        _notificacaoRepository = notificacaoRepository;
    }

    public async Task CriarNotificacaoAsync(
        Guid destinatarioId,
        Guid remetenteId,
        string tipo,
        long? postId = null,
        string? postTexto = null,
        string? comentarioTexto = null)
    {
        if (destinatarioId == remetenteId)
            return;

        var notificacao = new Notificacao
        {
            DestinatarioId = destinatarioId,
            RemetenteId = remetenteId,
            Tipo = tipo,
            Lida = false,
            PostId = postId,
            PostTexto = postTexto != null && postTexto.Length > 100
                ? postTexto[..100] + "..."
                : postTexto,
            ComentarioTexto = comentarioTexto,
            CriadoEm = DateTime.UtcNow,
        };

        await _notificacaoRepository.CriarAsync(notificacao);
    }

    public async Task<List<NotificacaoResponse>> ObterNotificacoesAsync(Guid usuarioId, int pagina, int limite)
    {
        var notificacoes = await _notificacaoRepository.ObterPorDestinatarioAsync(usuarioId, pagina, limite);
        return notificacoes.Select(MapearResponse).ToList();
    }

    public Task<int> ContarNaoLidasAsync(Guid usuarioId)
        => _notificacaoRepository.ContarNaoLidasAsync(usuarioId);

    public Task MarcarTodasComoLidasAsync(Guid usuarioId)
        => _notificacaoRepository.MarcarTodasComoLidasAsync(usuarioId);

    private static NotificacaoResponse MapearResponse(Notificacao n) => new()
    {
        Id = n.Id,
        Tipo = n.Tipo,
        Lida = n.Lida,
        CriadoEm = n.CriadoEm,
        RemetenteId = n.RemetenteId.ToString(),
        RemetenteNome = n.Remetente?.Nome ?? string.Empty,
        RemetenteFoto = n.Remetente?.FotoPerfil,
        PostId = n.PostId,
        PostTexto = n.PostTexto,
        ComentarioTexto = n.ComentarioTexto,
    };
}
