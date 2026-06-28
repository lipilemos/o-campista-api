using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services
{


    public class CheckinService : ICheckinService
    {
        private readonly ICheckinRepository _checkinRepository;
        private readonly ICampingRepository _campingRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IConquistaService _conquistaService;
        private readonly ISalaChatService _salaChatService;
        private readonly ILogger<CheckinService> _logger;

        public CheckinService(
            ICheckinRepository checkinRepository,
            ICampingRepository campingRepository,
            IUsuarioRepository usuarioRepository,
            IUsuarioService usuarioService,
            IConquistaService conquistaService,
            ISalaChatService salaChatService,
            ILogger<CheckinService> logger)
        {
            _checkinRepository = checkinRepository;
            _campingRepository = campingRepository;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _conquistaService = conquistaService;
            _salaChatService = salaChatService;
            _logger = logger;
        }

        public async Task RealizarCheckinAsync(
            CheckinRequest request)
        {
            _logger.LogInformation(
                "Iniciando check-in. Usuario={UsuarioId} Camping={CampingId}",
                request.UsuarioId,
                request.CampingId);

            var camping =
                await _campingRepository
                    .ObterPorIdAsync(
                        request.CampingId);

            if (camping is null)
            {
                throw new Exception(
                    "Camping não encontrado.");
            }

            var existeHoje =
                await _checkinRepository
                    .JaExisteHojeAsync(
                        request.UsuarioId,
                        request.CampingId);

            if (existeHoje)
            {
                throw new Exception(
                    "Você já realizou check-in neste camping hoje.");
            }

            var checkin = new Checkin
            {
                UsuarioId = request.UsuarioId,
                CampingId = request.CampingId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                XpGanho = 100,
                CriadoEm = DateTime.UtcNow
            };

            await _checkinRepository.CriarAsync(checkin);

            await _usuarioService.AdicionarXPAsync(request.UsuarioId, 100);

            await _conquistaService.VerificarConquistasAsync(request.UsuarioId);

            await _salaChatService.CriarSalaCampingSeNaoExisteAsync(request.CampingId, request.UsuarioId);

            _logger.LogInformation(
                "Check-in realizado com sucesso. Usuario={UsuarioId} Camping={CampingId}",
                request.UsuarioId,
                request.CampingId);
        }

        public async Task<List<HistoricoCheckinResponse>> ObtenerHistoricoAsync(Guid usuarioId)
        {
            var checkins = await _checkinRepository.ObtenerHistoricoAsync(usuarioId);

            return checkins.Select(c => new HistoricoCheckinResponse
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                CampingId = c.CampingId,
                Camping = new CampingInfoResponse
                {
                    Id = c.Camping.Id,
                    Nome = c.Camping.Nome,
                    Descricao = c.Camping.Descricao,
                    Latitude = c.Camping.Latitude,
                    Longitude = c.Camping.Longitude,
                    Cidade = c.Camping?.Cidade,
                    Estado = c.Camping?.Estado,
                    Endereco = c.Camping?.Endereco,
                    FotoPrincipal = c.Camping?.Fotos.FirstOrDefault().Url
                },
                DataCriacao = c.CriadoEm,
                Latitude = c.Latitude,
                Longitude = c.Longitude
            }).ToList();
        }

        public async Task<int> ContarPessoasUltimas24hAsync(long campingId)
        {
            return await _checkinRepository.ContarCheckinsUltimas24hAsync(campingId);
        }
    }
}
