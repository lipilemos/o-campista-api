using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;

namespace o_campista.business.imp.Services
{
    

    public class CheckinService : ICheckinService
    {
        private readonly ICheckinRepository _checkinRepository;
        private readonly ICampingRepository _campingRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<CheckinService> _logger;

        public CheckinService(
            ICheckinRepository checkinRepository,
            ICampingRepository campingRepository,
            IUsuarioRepository usuarioRepository,
            ILogger<CheckinService> logger)
        {
            _checkinRepository = checkinRepository;
            _campingRepository = campingRepository;
            _usuarioRepository = usuarioRepository;
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

            await _checkinRepository
                .CriarAsync(checkin);

            var usuario =
                await _usuarioRepository
                    .ObterPorIdAsync(
                        request.UsuarioId);

            if (usuario is not null)
            {
                usuario.XP += 100;

                await _usuarioRepository
                    .AtualizarAsync(usuario);
            }

            _logger.LogInformation(
                "Check-in realizado com sucesso. Usuario={UsuarioId} Camping={CampingId}",
                request.UsuarioId,
                request.CampingId);
        }
    }
}
