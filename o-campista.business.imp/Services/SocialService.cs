using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class SocialService : ISocialService
{
    private readonly ISocialRepository _socialRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly INotificacaoService _notificacaoService;
    private readonly ILogger<SocialService> _logger;

    public SocialService(
        ISocialRepository socialRepository,
        IUsuarioRepository usuarioRepository,
        INotificacaoService notificacaoService,
        ILogger<SocialService> logger)
    {
        _socialRepository = socialRepository;
        _usuarioRepository = usuarioRepository;
        _notificacaoService = notificacaoService;
        _logger = logger;
    }

    public async Task<PerfilPublicoResponse> ObterPerfilPublicoAsync(Guid perfilId, Guid? requisitanteId)
    {
        _logger.LogInformation("Buscando perfil público do usuário {PerfilId}", perfilId);

        var usuario = await _usuarioRepository.ObterPorIdComDetalhesAsync(perfilId);
        if (usuario is null)
            throw new Exception("Usuário não encontrado.");

        var privacidade = await _socialRepository.ObterPrivacidadeAsync(perfilId);
        var totalSeguidores = await _socialRepository.ContarSeguidoresAsync(perfilId);
        var totalSeguindo = await _socialRepository.ContarSeguindoAsync(perfilId);
        var estouSeguindo = requisitanteId.HasValue
            && await _socialRepository.EstaSeguidoAsync(requisitanteId.Value, perfilId);
        var segueMutuo = requisitanteId.HasValue
            && await _socialRepository.SegueMutuamenteAsync(requisitanteId.Value, perfilId);

        var nivelPublico = privacidade?.NivelPublico ?? true;
        var checkinsPublicos = privacidade?.CheckinsPublicos ?? true;
        var conquistasPublicas = privacidade?.ConquistasPublicas ?? true;

        return new PerfilPublicoResponse
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            FotoPerfil = usuario.FotoPerfil ?? string.Empty,
            Nivel = nivelPublico ? usuario.Nivel : null,
            Xp = nivelPublico ? usuario.XP : null,
            TotalCheckins = checkinsPublicos ? usuario.Checkins.Count : null,
            TotalCampingsVisitados = checkinsPublicos
                ? usuario.Checkins.Select(x => x.CampingId).Distinct().Count()
                : null,
            TotalTrilhasConcluidas = checkinsPublicos
                ? usuario.UsuarioTrilhas.Count(x => x.Concluida)
                : null,
            Conquistas = conquistasPublicas
                ? usuario.UsuarioConquistas
                    .OrderByDescending(c => c.CriadoEm)
                    .Take(10)
                    .Select(c => new ConquistaResponse
                    {
                        Id = c.Conquista.Id,
                        Nome = c.Conquista.Nome,
                        Descricao = c.Conquista.Descricao,
                        Icone = c.Conquista.Icone,
                        DataConquista = c.CriadoEm
                    })
                : null,
            UltimosCheckins = checkinsPublicos
                ? usuario.Checkins
                    .Where(c => c.CampingId.HasValue)
                    .OrderByDescending(c => c.CriadoEm)
                    .Take(5)
                    .Select(c => new HistoricoCheckinResponse
                    {
                        Id = c.Id,
                        UsuarioId = c.UsuarioId,
                        CampingId = c.CampingId ?? 0,
                        TrilhaId = c.TrilhaId,
                        DataCriacao = c.CriadoEm,
                        Latitude = (decimal)c.Latitude,
                        Longitude = (decimal)c.Longitude,
                        Camping = c.Camping != null
                            ? new CampingInfoResponse
                            {
                                Id = c.Camping.Id,
                                Nome = c.Camping.Nome,
                                Descricao = c.Camping.Descricao ?? string.Empty,
                                Latitude = c.Camping.Latitude,
                                Longitude = c.Camping.Longitude,
                                FotoPrincipal = c.Camping.Fotos
                                    .FirstOrDefault()?.Url ?? string.Empty
                            }
                            : null!
                    })
                : null,
            TotalSeguidores = totalSeguidores,
            TotalSeguindo = totalSeguindo,
            EstouSeguindo = estouSeguindo,
            SegueMutuo = segueMutuo
        };
    }

    public async Task<List<UsuarioBuscaResponse>> BuscarUsuariosAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new ArgumentException("O nome deve ter ao menos 2 caracteres.");

        var usuarios = await _socialRepository.BuscarPorNomeAsync(nome);
        return usuarios.Select(u => new UsuarioBuscaResponse
        {
            Id = u.Id,
            Nome = u.Nome,
            FotoPerfil = u.FotoPerfil ?? string.Empty,
            Nivel = u.Nivel
        }).ToList();
    }

    public async Task SegueAsync(Guid seguidorId, Guid seguidoId)
    {
        if (seguidorId == seguidoId)
            throw new ArgumentException("Você não pode seguir a si mesmo.");

        var jaSegue = await _socialRepository.EstaSeguidoAsync(seguidorId, seguidoId);
        if (jaSegue)
            throw new InvalidOperationException("Você já segue este usuário.");

        var alvo = await _usuarioRepository.ObterPorIdAsync(seguidoId);
        if (alvo is null || !alvo.Ativo)
            throw new Exception("Usuário não encontrado.");

        await _socialRepository.SegueAsync(new Seguidor
        {
            SeguidorId = seguidorId,
            SeguidoId = seguidoId,
            CriadoEm = DateTime.UtcNow
        });

        _ = _notificacaoService.CriarNotificacaoAsync(seguidoId, seguidorId, "novo_seguidor");
        _logger.LogInformation("{SeguidorId} começou a seguir {SeguidoId}", seguidorId, seguidoId);
    }

    public async Task PararDeSegueAsync(Guid seguidorId, Guid seguidoId)
    {
        await _socialRepository.PararDeSegueAsync(seguidorId, seguidoId);
        _logger.LogInformation("{SeguidorId} parou de seguir {SeguidoId}", seguidorId, seguidoId);
    }

    public async Task<List<UsuarioBuscaResponse>> ObterSeguidoresAsync(Guid usuarioId)
    {
        var usuarios = await _socialRepository.ObterSeguidoresAsync(usuarioId);
        return usuarios.Select(u => new UsuarioBuscaResponse
        {
            Id = u.Id,
            Nome = u.Nome,
            FotoPerfil = u.FotoPerfil ?? string.Empty,
            Nivel = u.Nivel
        }).ToList();
    }

    public async Task<List<UsuarioBuscaResponse>> ObterSeguindoAsync(Guid usuarioId)
    {
        var usuarios = await _socialRepository.ObterSeguindoAsync(usuarioId);
        return usuarios.Select(u => new UsuarioBuscaResponse
        {
            Id = u.Id,
            Nome = u.Nome,
            FotoPerfil = u.FotoPerfil ?? string.Empty,
            Nivel = u.Nivel
        }).ToList();
    }

    public async Task<ConfiguracaoPrivacidadeResponse> ObterPrivacidadeAsync(Guid usuarioId)
    {
        var config = await _socialRepository.ObterPrivacidadeAsync(usuarioId);

        // Retorna defaults públicos se ainda não configurado
        return new ConfiguracaoPrivacidadeResponse
        {
            PerfilPublico = config?.PerfilPublico ?? true,
            CheckinsPublicos = config?.CheckinsPublicos ?? true,
            ConquistasPublicas = config?.ConquistasPublicas ?? true,
            NivelPublico = config?.NivelPublico ?? true
        };
    }

    public async Task SalvarPrivacidadeAsync(Guid usuarioId, ConfiguracaoPrivacidadeRequest request)
    {
        await _socialRepository.SalvarPrivacidadeAsync(new ConfiguracaoPrivacidade
        {
            UsuarioId = usuarioId,
            PerfilPublico = request.PerfilPublico,
            CheckinsPublicos = request.CheckinsPublicos,
            ConquistasPublicas = request.ConquistasPublicas,
            NivelPublico = request.NivelPublico
        });

        _logger.LogInformation("Privacidade atualizada para usuário {UsuarioId}", usuarioId);
    }

    public async Task<List<UsuarioBuscaResponse>> ObterSugestoesAsync(Guid usuarioId)
    {
        var usuarios = await _socialRepository.ObterSugestoesAsync(usuarioId);
        return usuarios.Select(u => new UsuarioBuscaResponse
        {
            Id = u.Id,
            Nome = u.Nome,
            FotoPerfil = u.FotoPerfil ?? string.Empty,
            Nivel = u.Nivel
        }).ToList();
    }
}
