using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services
{
    public class CampingAvaliacaoService : ICampingAvaliacaoService
    {
        private readonly ICampingAvaliacaoRepository _avaliacaoRepository;
        private readonly ICampingRepository _campingRepository;
        private readonly ICheckinRepository _checkinRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IConquistaService _conquistaService;
        private readonly ILogger<CampingAvaliacaoService> _logger;

        public CampingAvaliacaoService(
            ICampingAvaliacaoRepository avaliacaoRepository,
            ICampingRepository campingRepository,
            ICheckinRepository checkinRepository,
            IUsuarioService usuarioService,
            IConquistaService conquistaService,
            ILogger<CampingAvaliacaoService> logger)
        {
            _avaliacaoRepository = avaliacaoRepository;
            _campingRepository = campingRepository;
            _checkinRepository = checkinRepository;
            _usuarioService = usuarioService;
            _conquistaService = conquistaService;
            _logger = logger;
        }

        public async Task<CampingAvaliacaoResponse> CriarAvaliacaoAsync(CampingAvaliacaoRequest request)
        {
            _logger.LogInformation(
                "Criando avaliação. Usuario={UsuarioId} Camping={CampingId}",
                request.UsuarioId,
                request.CampingId);

            if (request.Nota < 1 || request.Nota > 5)
            {
                throw new ArgumentException("A nota deve estar entre 1 e 5.");
            }

            if (string.IsNullOrWhiteSpace(request.Comentario))
            {
                throw new ArgumentException("O comentário é obrigatório.");
            }

            var avaliacaoExistente = await _avaliacaoRepository.ObterPorCheckinAsync(request.CheckinId);
            if (avaliacaoExistente != null)
            {
                throw new Exception("Já existe uma avaliação para este check-in.");
            }

            var camping = await _campingRepository.ObterPorIdAsync(request.CampingId);
            if (camping == null)
            {
                throw new Exception("Camping não encontrado.");
            }

            var avaliacao = new CampingAvaliacao
            {
                CampingId = request.CampingId,
                UsuarioId = request.UsuarioId,
                CheckinId = request.CheckinId,
                Nota = request.Nota,
                Comentario = request.Comentario,
                CriadoEm = DateTime.UtcNow
            };

            await _avaliacaoRepository.CriarAsync(avaliacao);

            await _usuarioService.AdicionarXPAsync(request.UsuarioId, request.XpGanho);
            await _conquistaService.VerificarConquistasAsync(request.UsuarioId);

            await AtualizarMediaCampingAsync(request.CampingId);

            _logger.LogInformation(
                "Avaliação criada com sucesso. Avaliacao={AvaliacaoId}",
                avaliacao.Id);

            return new CampingAvaliacaoResponse
            {
                Id = avaliacao.Id,
                UsuarioId = avaliacao.UsuarioId,
                CampingId = avaliacao.CampingId,
                Nota = avaliacao.Nota,
                Comentario = avaliacao.Comentario,
                DataCriacao = avaliacao.CriadoEm,
            };
        }

        public async Task<CampingAvaliacaoResponse?> ObterPorIdAsync(long id)
        {
            var avaliacao = await _avaliacaoRepository.ObterPorIdAsync(id);
            if (avaliacao == null)
                return null;

            return new CampingAvaliacaoResponse
            {
                Id = avaliacao.Id,
                UsuarioId = avaliacao.UsuarioId,
                CampingId = avaliacao.CampingId,
                Nota = avaliacao.Nota,
                Comentario = avaliacao.Comentario,
                DataCriacao = avaliacao.CriadoEm,
            };
        }

        public async Task<List<CampingAvaliacaoComUsuarioResponse>> ObterAvaliacoesCampingAsync(long campingId)
        {
            _logger.LogInformation("Obtendo avaliações do camping. CampingId={CampingId}", campingId);

            var avaliacoes = await _avaliacaoRepository.ObterPorCampingAsync(campingId);

            return avaliacoes.Select(a => new CampingAvaliacaoComUsuarioResponse
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                CampingId = a.CampingId,
                Nota = a.Nota,
                Comentario = a.Comentario,
                DataCriacao = a.CriadoEm,
                UsuarioNome = a.Usuario.Nome,
                UsuarioFoto = string.Empty
            }).ToList();
        }

        public async Task<List<CampingAvaliacaoComUsuarioResponse>> ObterAvaliacoesPorUsuarioAsync(long campingId, Guid usuarioId, long? checkinId = null)
        {
            _logger.LogInformation(
                "Obtendo avaliações do usuario no camping. CampingId={CampingId} UsuarioId={UsuarioId} CheckinId={CheckinId}",
                campingId,
                usuarioId,
                checkinId);

            var avaliacoes = await _avaliacaoRepository.ObterPorCampingEUsuarioAsync(campingId, usuarioId, checkinId);

            return avaliacoes.Select(a => new CampingAvaliacaoComUsuarioResponse
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                CampingId = a.CampingId,
                Nota = a.Nota,
                Comentario = a.Comentario,
                CheckinId = a.CheckinId,
                DataCriacao = a.CriadoEm,
                UsuarioNome = a.Usuario.Nome,
                UsuarioFoto = string.Empty
            }).ToList();
        }

        public async Task AtualizarAvaliacaoAsync(long id, CampingAvaliacaoRequest request)
        {
            _logger.LogInformation("Atualizando avaliação. AvaliacaoId={AvaliacaoId}", id);

            if (request.Nota < 1 || request.Nota > 5)
            {
                throw new ArgumentException("A nota deve estar entre 1 e 5.");
            }

            if (string.IsNullOrWhiteSpace(request.Comentario))
            {
                throw new ArgumentException("O comentário é obrigatório.");
            }

            var avaliacao = await _avaliacaoRepository.ObterPorIdAsync(id);
            if (avaliacao == null)
            {
                throw new Exception("Avaliação não encontrada.");
            }

            var notaAnterior = avaliacao.Nota;
            avaliacao.Nota = request.Nota;
            avaliacao.Comentario = request.Comentario;

            await _avaliacaoRepository.AtualizarAsync(avaliacao);

            if (notaAnterior != request.Nota)
            {
                await AtualizarMediaCampingAsync(avaliacao.CampingId);
            }

            _logger.LogInformation("Avaliação atualizada com sucesso. AvaliacaoId={AvaliacaoId}", id);
        }

        public async Task DeletarAvaliacaoAsync(long id)
        {
            _logger.LogInformation("Deletando avaliação. AvaliacaoId={AvaliacaoId}", id);

            var avaliacao = await _avaliacaoRepository.ObterPorIdAsync(id);
            if (avaliacao == null)
            {
                throw new Exception("Avaliação não encontrada.");
            }

            var campingId = avaliacao.CampingId;
            await _avaliacaoRepository.DeletarAsync(id);

            await AtualizarMediaCampingAsync(campingId);

            _logger.LogInformation("Avaliação deletada com sucesso. AvaliacaoId={AvaliacaoId}", id);
        }

        private async Task AtualizarMediaCampingAsync(long campingId)
        {
            var avaliacoes = await _avaliacaoRepository.ObterPorCampingAsync(campingId);

            if (avaliacoes.Count == 0)
            {
                return;
            }

            var mediaNota = (decimal)avaliacoes.Average(a => a.Nota);

            await _campingRepository.AtualizarMediaAvaliacaoAsync(campingId, mediaNota);

            _logger.LogInformation(
                "Média de avaliação atualizada. CampingId={CampingId} NovaMedia={NovaMedia}",
                campingId,
                mediaNota);
        }
    }
}
