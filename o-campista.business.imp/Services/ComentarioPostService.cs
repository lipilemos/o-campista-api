using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class ComentarioPostService : IComentarioPostService
{
    private readonly IComentarioPostRepository _comentarioRepository;
    private readonly IPostRepository _postRepository;
    private readonly INotificacaoService _notificacaoService;
    private readonly ILogger<ComentarioPostService> _logger;

    public ComentarioPostService(
        IComentarioPostRepository comentarioRepository,
        IPostRepository postRepository,
        INotificacaoService notificacaoService,
        ILogger<ComentarioPostService> logger)
    {
        _comentarioRepository = comentarioRepository;
        _postRepository = postRepository;
        _notificacaoService = notificacaoService;
        _logger = logger;
    }

    public async Task<ComentarioPostResponse> CriarComentarioAsync(long postId, Guid usuarioId, ComentarioPostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
            throw new ArgumentException("O texto do comentário é obrigatório.");

        if (request.Texto.Length > 500)
            throw new ArgumentException("O comentário não pode ter mais de 500 caracteres.");

        var post = await _postRepository.ObterPorIdAsync(postId)
            ?? throw new Exception("Post não encontrado.");

        _logger.LogInformation("Criando comentário. PostId={PostId} UsuarioId={UsuarioId}", postId, usuarioId);

        var comentario = new ComentarioPost
        {
            PostId = postId,
            UsuarioId = usuarioId,
            Texto = request.Texto.Trim(),
            CriadoEm = DateTime.UtcNow
        };

        var criado = await _comentarioRepository.CriarAsync(comentario);

        // Recarregar com dados do usuário para montar o response
        var comUsuario = await _comentarioRepository.ObterPorIdAsync(criado.Id);

        _ = _notificacaoService.CriarNotificacaoAsync(
            post.UsuarioId, usuarioId, "novo_comentario",
            postId: postId, postTexto: post.Texto, comentarioTexto: request.Texto.Trim());

        return MapearResponse(comUsuario!);
    }

    public async Task DeletarComentarioAsync(long comentarioId, Guid usuarioId)
    {
        var comentario = await _comentarioRepository.ObterPorIdAsync(comentarioId)
            ?? throw new Exception("Comentário não encontrado.");

        if (comentario.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Você não tem permissão para deletar este comentário.");

        await _comentarioRepository.DeletarAsync(comentarioId);
        _logger.LogInformation("Comentário deletado. ComentarioId={ComentarioId}", comentarioId);
    }

    public async Task<List<ComentarioPostResponse>> ObterComentariosAsync(long postId, int pagina, int limite)
    {
        var comentarios = await _comentarioRepository.ObterPorPostAsync(postId, pagina, limite);
        return comentarios.Select(MapearResponse).ToList();
    }

    private static ComentarioPostResponse MapearResponse(ComentarioPost c) => new()
    {
        Id = c.Id,
        PostId = c.PostId,
        UsuarioId = c.UsuarioId,
        UsuarioNome = c.Usuario?.Nome ?? string.Empty,
        UsuarioFoto = c.Usuario?.FotoPerfil,
        Texto = c.Texto,
        CriadoEm = c.CriadoEm
    };
}
