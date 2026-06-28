using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Enums;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace o_campista.business.imp.Services;


public class PresenteService : IPresenteService
{
    private readonly IPresenteRepository _repository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IStorageService _storageService;
    private readonly IUsuarioPresenteRepository _usuarioPresenteRepository;
    private readonly IConquistaService _conquistaService;
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<PresenteService> _logger;

    public PresenteService(
        IPresenteRepository repository,
        IUsuarioRepository usuarioRepository,
        IUsuarioService usuarioService,
        IUsuarioPresenteRepository usuarioPresenteRepository,
        IStorageService storageService,
        IConquistaService conquistaService,
        ILogger<PresenteService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _repository = repository;
        _storageService = storageService;
        _usuarioService = usuarioService;
        _conquistaService = conquistaService;
        _usuarioPresenteRepository = usuarioPresenteRepository;
        _logger = logger;
    }

    public async Task<Presente> CriarNovoPresenteAsync(PresenteCreateRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Iniciando criação de presente. Usuário: {UsuarioId}",
                request.UsuarioCriadorId);

            var latitude = double.Parse(
                request.Latitude,
                CultureInfo.InvariantCulture);

            var longitude = double.Parse(
                request.Longitude,
                CultureInfo.InvariantCulture);

            _logger.LogInformation(
                "Coordenadas recebidas. Latitude: {Latitude}, Longitude: {Longitude}",
                latitude,
                longitude);

            var urlFoto =
                await _storageService.UploadAsync(request.Foto, BucketTypeEnum.BucketGift);

            _logger.LogInformation(
                "Upload da foto concluído. Url: {UrlFoto}",
                urlFoto);

            var codigoResgate =
                GenerateCodigoResgate(
                    request.UsuarioCriadorId,
                    latitude,
                    longitude);

            _logger.LogDebug(
                "Código de resgate gerado para o presente.");

            var presente = new Presente
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                FotoUrl = urlFoto,
                CodigoResgate = codigoResgate,
                UsuarioCriadorId = request.UsuarioCriadorId,
                Location = new Point(longitude, latitude)
                {
                    SRID = 4326
                },
                EstaDisponivel = true,
                CriadoEm = DateTime.UtcNow
            };

            await _repository.CriarAsync(presente);
            
            await _usuarioService.AdicionarXPAsync(request.UsuarioCriadorId, 100);

            await _conquistaService.VerificarConquistasAsync(request.UsuarioCriadorId);

            _logger.LogInformation(
                "Presente criado. Id: {PresenteId}, Usuario: {UsuarioId}, Nome: {Nome}",
                presente.Id,
                request.UsuarioCriadorId,
                presente.Nome);

            return presente;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao criar presente para usuário {UsuarioId}",
                request.UsuarioCriadorId);

            throw;
        }
    }

    public async Task<IEnumerable<PresenteResponse>>ObterPresentesPorRaioAsync(double latitude,double longitude)
    {
        try
        {
            _logger.LogInformation(
                "Buscando presentes próximos. Latitude: {Latitude}, Longitude: {Longitude}",
                latitude,
                longitude);

            var presentes =
                await _repository
                    .ObterPresentesPorRaioAsync(
                        latitude,
                        longitude);

            var resultado =
                presentes.Select(p => new PresenteResponse
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao ?? string.Empty,
                    CodigoResgate = p.CodigoResgate ?? string.Empty,
                    FotoUrl = p.FotoUrl ?? string.Empty,
                    Latitude = p.Location?.Y ?? 0,
                    Longitude = p.Location?.X ?? 0,
                    UsuarioCriadorId = p.UsuarioCriadorId,
                    EstaDisponivel = p.EstaDisponivel,
                    CriadoEm = p.CriadoEm,
                    Utilizado = false
                }).Where(x=>x.EstaDisponivel == true);

            _logger.LogInformation(
                "Busca concluída. {Quantidade} presentes encontrados.",
                resultado.Count());

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao buscar presentes próximos.");

            throw;
        }
    }
    public async Task ResgatarAsync(ResgatarPresenteRequest request)
    {
        _logger.LogInformation(
            "Iniciando resgate do presente {PresenteId} pelo usuário {UsuarioId}",
            request.PresenteId,
            request.UsuarioId);

        var presente = await _repository.ObterPorIdAsync(request.PresenteId);

        if (presente == null)
            throw new Exception("Presente não encontrado.");

        if (!presente.EstaDisponivel)
            throw new Exception("Este presente já foi resgatado.");

        if (presente.UsuarioCriadorId == request.UsuarioId)
            throw new Exception("Você não pode resgatar o seu próprio presente.");

        // 1 - Salva o vínculo usuário x presente
        var usuarioPresente = new UsuarioPresente
        {
            UsuarioId = request.UsuarioId,
            PresenteId = presente.Id,
            Utilizado = false,
            CriadoEm = DateTime.UtcNow
        };

        await _usuarioPresenteRepository.CriarAsync(usuarioPresente);

        await _repository.MarcarComoResgatadoAsync(presente);

        await _usuarioService.AdicionarXPAsync(request.UsuarioId,100);

        // 4 - Verifica conquistas
        await _conquistaService.VerificarConquistasAsync(request.UsuarioId);


        _logger.LogInformation(
            "Presente {PresenteId} resgatado com sucesso pelo usuário {UsuarioId}",
            request.PresenteId,
            request.UsuarioId);
    }
    public async Task DeletarAsync(long presenteId, Guid usuarioId)
    {
        var presente = await _repository.ObterPorIdAsync(presenteId);

        if (presente is null)
            throw new Exception("Presente não encontrado.");

        if (presente.UsuarioCriadorId != usuarioId)
            throw new Exception("Você só pode deletar presentes que criou.");

        if (!presente.EstaDisponivel)
            throw new Exception("Não é possível deletar um presente já resgatado.");

        await _repository.DeletarAsync(presente);
    }

    private static string GenerateCodigoResgate(Guid usuarioCriadorId,double latitude,double longitude)
    {
        var payload =
            $"{usuarioCriadorId:N}:{latitude.ToString(CultureInfo.InvariantCulture)}:{longitude.ToString(CultureInfo.InvariantCulture)}";

        using var hmac =
            new HMACSHA256(
                Encoding.UTF8.GetBytes(
                    "o-campista-secret-key"));

        var hash =
            hmac.ComputeHash(
                Encoding.UTF8.GetBytes(payload));

        return Convert.ToBase64String(hash)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

}