using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Enums;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;
using System.Net;

namespace o_campista.business.imp.Services;

public class AchadoPerdidoService : IAchadoPerdidoService
{
    private const string TipoAchado = "achado";
    private const string TipoPerdido = "perdido";

    private readonly IAchadoPerdidoRepository _repository;
    private readonly ICheckinRepository _checkinRepository;
    private readonly ISalaChatService _salaChatService;
    private readonly IStorageService _storageService;
    private readonly ILogger<AchadoPerdidoService> _logger;

    public AchadoPerdidoService(
        IAchadoPerdidoRepository repository,
        ICheckinRepository checkinRepository,
        ISalaChatService salaChatService,
        IStorageService storageService,
        ILogger<AchadoPerdidoService> logger)
    {
        _repository = repository;
        _checkinRepository = checkinRepository;
        _salaChatService = salaChatService;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<AcessoAchadosResponse> ObterAcessoAsync(Guid usuarioId, long campingId)
    {
        var podePublicar = await _checkinRepository.TemCheckinNasUltimas24hAsync(usuarioId, campingId);

        return new AcessoAchadosResponse
        {
            // Quem pode publicar necessariamente já fez check-in, então poupamos a segunda query.
            PodeVer = podePublicar || await _checkinRepository.JaFezCheckinNoCampingAsync(usuarioId, campingId),
            PodePublicar = podePublicar
        };
    }

    public async Task<List<AchadoPerdidoResponse>> ListarAsync(
        Guid usuarioId, long campingId, string? tipo, bool incluirResolvidos, int pagina, int limite)
    {
        var podeVer = await _checkinRepository.JaFezCheckinNoCampingAsync(usuarioId, campingId);
        if (!podeVer)
            throw new UnauthorizedAccessException(
                "Faça check-in neste camping para ver o mural de achados e perdidos.");

        if (!string.IsNullOrWhiteSpace(tipo) && tipo != TipoAchado && tipo != TipoPerdido)
            throw new ArgumentException("Tipo inválido. Use 'achado' ou 'perdido'.");

        if (pagina < 1) pagina = 1;
        if (limite < 1 || limite > 50) limite = 20;

        var itens = await _repository.ListarPorCampingAsync(campingId, tipo, incluirResolvidos, pagina, limite);

        return itens.Select(item => Mapear(item, usuarioId)).ToList();
    }

    public async Task<AchadoPerdidoResponse> CriarAsync(
        Guid usuarioId, long campingId, AchadoPerdidoRequest request)
    {
        var podePublicar = await _checkinRepository.TemCheckinNasUltimas24hAsync(usuarioId, campingId);
        if (!podePublicar)
            throw new UnauthorizedAccessException(
                "Você precisa ter feito check-in neste camping nas últimas 24 horas para publicar.");

        var tipo = (request.Tipo ?? string.Empty).Trim().ToLowerInvariant();
        if (tipo != TipoAchado && tipo != TipoPerdido)
            throw new ArgumentException("Tipo inválido. Use 'achado' ou 'perdido'.");

        var titulo = (request.Titulo ?? string.Empty).Trim();
        if (titulo.Length < 3 || titulo.Length > 120)
            throw new ArgumentException("O título deve ter entre 3 e 120 caracteres.");

        var descricao = request.Descricao?.Trim();
        if (descricao?.Length > 500)
            throw new ArgumentException("A descrição não pode ter mais de 500 caracteres.");

        var localGuarda = request.LocalGuarda?.Trim();
        if (localGuarda?.Length > 120)
            throw new ArgumentException("O local de guarda não pode ter mais de 120 caracteres.");

        // A foto prova que o item existe e ajuda o dono a reconhecê-lo — obrigatória para "achado".
        if (tipo == TipoAchado && request.Foto is null)
            throw new ArgumentException("A foto é obrigatória para itens achados.");

        _logger.LogInformation(
            "Criando item de achados e perdidos. Usuario={UsuarioId} Camping={CampingId} Tipo={Tipo}",
            usuarioId, campingId, tipo);

        string? fotoUrl = null;
        if (request.Foto is not null)
            fotoUrl = await _storageService.UploadAsync(request.Foto, BucketTypeEnum.BucketAchadosPerdidos);

        var item = new AchadoPerdido
        {
            CampingId = campingId,
            UsuarioId = usuarioId,
            Tipo = tipo,
            Titulo = WebUtility.HtmlEncode(titulo),
            Descricao = string.IsNullOrEmpty(descricao) ? null : WebUtility.HtmlEncode(descricao),
            LocalGuarda = string.IsNullOrEmpty(localGuarda) ? null : WebUtility.HtmlEncode(localGuarda),
            FotoUrl = fotoUrl,
            Resolvido = false,
            CriadoEm = DateTime.UtcNow
        };

        var criado = await _repository.CriarAsync(item);
        _logger.LogInformation("Item de achados e perdidos criado. Id={Id}", criado.Id);

        // Recarrega para trazer o usuário e devolver o card já completo ao front.
        var completo = await _repository.ObterPorIdAsync(criado.Id) ?? criado;
        return Mapear(completo, usuarioId);
    }

    public async Task ResolverAsync(long id, Guid usuarioId)
    {
        var item = await ObterDoAutorAsync(id, usuarioId);

        if (item.Resolvido)
            return;

        item.Resolvido = true;
        item.ResolvidoEm = DateTime.UtcNow;
        await _repository.AtualizarAsync(item);

        _logger.LogInformation("Item de achados e perdidos resolvido. Id={Id}", id);
    }

    public async Task DeletarAsync(long id, Guid usuarioId)
    {
        var item = await ObterDoAutorAsync(id, usuarioId);
        await _repository.DeletarAsync(item);

        _logger.LogInformation("Item de achados e perdidos removido. Id={Id}", id);
    }

    public async Task<SalaChatResponse> ReivindicarAsync(long id, Guid usuarioId)
    {
        var item = await _repository.ObterPorIdAsync(id)
            ?? throw new Exception("Item não encontrado.");

        if (item.UsuarioId == usuarioId)
            throw new ArgumentException("Este item foi publicado por você.");

        if (item.Resolvido)
            throw new InvalidOperationException("Este item já foi resolvido.");

        var podeVer = await _checkinRepository.JaFezCheckinNoCampingAsync(usuarioId, item.CampingId);
        if (!podeVer)
            throw new UnauthorizedAccessException(
                "Faça check-in neste camping para falar sobre este item.");

        // O item aberto é o que autoriza a DM — ver ExisteVinculoAtivoAsync.
        return await _salaChatService.ObterOuCriarDmAsync(usuarioId, item.UsuarioId);
    }

    private async Task<AchadoPerdido> ObterDoAutorAsync(long id, Guid usuarioId)
    {
        var item = await _repository.ObterPorIdAsync(id)
            ?? throw new Exception("Item não encontrado.");

        if (item.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Você não tem permissão para alterar este item.");

        return item;
    }

    private static AchadoPerdidoResponse Mapear(AchadoPerdido item, Guid usuarioId) => new()
    {
        Id = item.Id,
        CampingId = item.CampingId,
        Tipo = item.Tipo,
        Titulo = item.Titulo,
        Descricao = item.Descricao,
        FotoUrl = item.FotoUrl,
        LocalGuarda = item.LocalGuarda,
        Resolvido = item.Resolvido,
        CriadoEm = item.CriadoEm,
        UsuarioId = item.UsuarioId,
        UsuarioNome = item.Usuario?.Nome ?? string.Empty,
        UsuarioFoto = item.Usuario?.FotoPerfil,
        SouAutor = item.UsuarioId == usuarioId
    };
}
