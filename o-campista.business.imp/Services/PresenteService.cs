using NetTopologySuite.Geometries;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace o_campista.business.imp.Services;


public class PresenteService : IPresenteService
{
    private readonly IPresenteRepository _repository;
    private readonly IStorageService _storageService;

    public PresenteService(IPresenteRepository repository, IStorageService storageService)
    {
        _repository = repository;
        _storageService = storageService;
    }

    public async Task<Presente> CriarNovoPresenteAsync(PresenteCreateRequest request)
    {
        var latitude = double.Parse(
                    request.Latitude,
                    CultureInfo.InvariantCulture);

        var longitude = double.Parse(
            request.Longitude,
            CultureInfo.InvariantCulture);

        var urlFoto =
        await _storageService.UploadAsync(
            request.Foto
        );


        var presente = new Presente
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            FotoUrl = urlFoto,
            CodigoResgate = GenerateCodigoResgate(request.UsuarioCriadorId, latitude, longitude),
            UsuarioCriadorId = request.UsuarioCriadorId,
            // Criando o ponto geográfico (Longitude, Latitude) para o PostGIS
            Location = new Point(longitude, latitude) { SRID = 4326 },
            EstaDisponivel = true,
            CriadoEm = DateTime.UtcNow
        };

        await _repository.CriarAsync(presente);
        return presente;
    }

    public async Task<IEnumerable<PresenteResponse>> ObterPresentesPorRaioAsync(double latitude, double longitude)
    {
        var presentes = await _repository.ObterPresentesPorRaioAsync(latitude, longitude);

        return presentes.Select(p => new PresenteResponse
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao ?? string.Empty,
            CodigoResgate = p.CodigoResgate ?? string.Empty,
            FotoUrl = p.FotoUrl ?? string.Empty,
            Latitude = p.Location?.Y ?? 0, // Latitude é a coordenada Y
            Longitude = p.Location?.X ?? 0, // Longitude é a coordenada X
            UsuarioCriadorId = p.UsuarioCriadorId,
            EstaDisponivel = p.EstaDisponivel,
            CriadoEm = p.CriadoEm,
            Utilizado = false
        });
    }

    private static string GenerateCodigoResgate(Guid usuarioCriadorId, double latitude, double longitude)
    {
        // Combine values into a canonical string
        var payload = $"{usuarioCriadorId:N}:{latitude.ToString(CultureInfo.InvariantCulture)}:{longitude.ToString(CultureInfo.InvariantCulture)}";

        // Use a random key per runtime; for deterministic but unique codes you could use a fixed server key from config
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("o-campista-secret-key"));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

        // Return URL-safe Base64 string without padding
        return Convert.ToBase64String(hash)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}