using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Enums;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IStorageService _storageService;
    private readonly IFeedService _feedService;
    private readonly INotificacaoService _notificacaoService;
    private readonly ILogger<PostService> _logger;

    public PostService(
        IPostRepository postRepository,
        IStorageService storageService,
        IFeedService feedService,
        INotificacaoService notificacaoService,
        ILogger<PostService> logger)
    {
        _postRepository = postRepository;
        _storageService = storageService;
        _feedService = feedService;
        _notificacaoService = notificacaoService;
        _logger = logger;
    }

    public async Task<PostViagemResponse> CriarPostAsync(Guid usuarioId, PostViagemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
            throw new ArgumentException("O texto do post é obrigatório.");

        if (request.Texto.Length > 1000)
            throw new ArgumentException("O texto não pode ter mais de 1000 caracteres.");

        _logger.LogInformation("Criando post. Usuario={UsuarioId}", usuarioId);

        string? fotoUrl = null;
        if (request.Foto != null)
            fotoUrl = await _storageService.UploadAsync(request.Foto, BucketTypeEnum.BucketPost);

        var post = new PostViagem
        {
            UsuarioId = usuarioId,
            Texto = request.Texto,
            FotoUrl = fotoUrl,
            CampingId = request.CampingId,
            TrilhaId = request.TrilhaId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CriadoEm = DateTime.UtcNow
        };

        var criado = await _postRepository.CriarAsync(post);

        await _feedService.CriarAtividadeAsync(usuarioId, "post", criado.Id);

        _logger.LogInformation("Post criado. PostId={PostId}", criado.Id);

        return new PostViagemResponse
        {
            Id = criado.Id,
            UsuarioId = usuarioId,
            Texto = criado.Texto,
            FotoUrl = criado.FotoUrl,
            CampingId = criado.CampingId,
            TrilhaId = criado.TrilhaId,
            CriadoEm = criado.CriadoEm,
            TotalCurtidas = 0,
            Curtiu = false
        };
    }

    public async Task DeletarPostAsync(long postId, Guid usuarioId)
    {
        var post = await _postRepository.ObterPorIdAsync(postId)
            ?? throw new Exception("Post não encontrado.");

        if (post.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Você não tem permissão para deletar este post.");

        await _postRepository.DeletarAsync(postId);
        _logger.LogInformation("Post deletado. PostId={PostId}", postId);
    }

    public async Task CurtirAsync(long postId, Guid usuarioId)
    {
        var post = await _postRepository.ObterPorIdAsync(postId)
            ?? throw new Exception("Post não encontrado.");

        var jaCurtiu = await _postRepository.JaCurtiuAsync(postId, usuarioId);
        if (jaCurtiu)
            throw new InvalidOperationException("Você já curtiu este post.");

        await _postRepository.CurtirAsync(new CurtidaPost
        {
            PostId = postId,
            UsuarioId = usuarioId,
            CriadoEm = DateTime.UtcNow
        });

        _ = _notificacaoService.CriarNotificacaoAsync(
            post.UsuarioId, usuarioId, "nova_curtida",
            postId: postId, postTexto: post.Texto);
    }

    public async Task DescurtirAsync(long postId, Guid usuarioId)
    {
        var post = await _postRepository.ObterPorIdAsync(postId)
            ?? throw new Exception("Post não encontrado.");

        await _postRepository.DescurtirAsync(postId, usuarioId);
    }

    public async Task<CurtidasPostResponse> ObterCurtidasAsync(long postId, Guid usuarioId)
    {
        var total = await _postRepository.ContarCurtidasAsync(postId);
        var curtiu = await _postRepository.JaCurtiuAsync(postId, usuarioId);
        return new CurtidasPostResponse { Total = total, Curtiu = curtiu };
    }
}
