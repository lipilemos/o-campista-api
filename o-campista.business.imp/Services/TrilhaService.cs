using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services
{
    public class TrilhaService : ITrilhaService
    {
        private readonly ITrilhaRepository _trilhaRepository;
        private readonly IUsuarioTrilhaRepository _usuarioTrilhaRepository;
        private readonly ICheckinRepository _checkinRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IConquistaService _conquistaService;

        public TrilhaService(
            ITrilhaRepository trilhaRepository,
            IUsuarioTrilhaRepository usuarioTrilhaRepository,
            ICheckinRepository checkinRepository,
            IUsuarioService usuarioService,
            IConquistaService conquistaService)
        {
            _trilhaRepository = trilhaRepository;
            _usuarioTrilhaRepository = usuarioTrilhaRepository;
            _checkinRepository = checkinRepository;
            _usuarioService = usuarioService;
            _conquistaService = conquistaService;
        }

        public async Task<IEnumerable<TrilhaResponse>> ObterPorCampingAsync(long campingId, Guid? usuarioId = null)
        {
            var trilhas = await _trilhaRepository.ObterPorCampingAsync(campingId);

            var responses = new List<TrilhaResponse>();
            foreach (var t in trilhas)
            {
                bool concluida = false;
                if (usuarioId.HasValue)
                {
                    var ut = await _usuarioTrilhaRepository.ObterAsync(usuarioId.Value, t.Id);
                    concluida = ut?.Concluida == true;
                }

                responses.Add(MapearResponse(t, concluida));
            }

            return responses;
        }

        public async Task<TrilhaResponse?> ObterDetalheAsync(long trilhaId, Guid? usuarioId = null)
        {
            var t = await _trilhaRepository.ObterPorIdComPontosAsync(trilhaId);
            if (t is null) return null;

            bool concluida = false;
            if (usuarioId.HasValue)
            {
                var ut = await _usuarioTrilhaRepository.ObterAsync(usuarioId.Value, t.Id);
                concluida = ut?.Concluida == true;
            }

            return MapearResponse(t, concluida);
        }

        public async Task<TrilhaResponse> CriarAsync(CriarTrilhaRequest request)
        {
            if (request.Pontos == null || request.Pontos.Count < 2)
                throw new ArgumentException("A trilha deve ter pelo menos 2 pontos GPS.");

            var pontosOrdenados = request.Pontos.OrderBy(p => p.Ordem).ToList();

            var trilha = new Trilha
            {
                CriadorId = request.CriadorId,
                CriadorNome = request.CriadorNome,
                Nome = request.Nome,
                Descricao = request.Descricao,
                Dificuldade = request.Dificuldade,
                DistanciaKm = CalcularDistanciaKm(pontosOrdenados),
                Latitude = pontosOrdenados[0].Latitude,
                Longitude = pontosOrdenados[0].Longitude,
                AvaliacaoMedia = 0,
                CriadoEm = DateTime.UtcNow,
                Pontos = pontosOrdenados.Select((p, i) => new TrilhaPonto
                {
                    Ordem = i,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude
                }).ToList()
            };

            var criada = await _trilhaRepository.CriarAsync(trilha);

            var registroExistente = await _usuarioTrilhaRepository.ObterAsync(request.CriadorId, criada.Id);
            if (registroExistente is null)
            {
                await _usuarioTrilhaRepository.CriarAsync(new UsuarioTrilha
                {
                    UsuarioId = request.CriadorId,
                    TrilhaId = criada.Id,
                    Concluida = true,
                    CriadoEm = DateTime.UtcNow,
                    ConcluidaEm = DateTime.UtcNow
                });
            }

            await _usuarioService.AdicionarXPAsync(request.CriadorId, 500);
            await _conquistaService.VerificarConquistasAsync(request.CriadorId);

            return MapearResponse(criada, true);
        }

        public async Task<IEnumerable<TrilhaResponse>> ObterIndependentesParaMapaAsync()
        {
            var trilhas = await _trilhaRepository.ObterIndependentesAsync();

            var responses = new List<TrilhaResponse>();
            foreach (var t in trilhas)
            {
                var checkinsRecentes = await _checkinRepository.ContarCheckinsUltimas24hTrilhaAsync(t.Id);
                var response = MapearResponse(t, false);
                response.CheckinsRecentes = checkinsRecentes;
                responses.Add(response);
            }

            return responses;
        }

        private static TrilhaResponse MapearResponse(Trilha t, bool concluida) =>
            new()
            {
                Id = t.Id,
                CampingId = t.CampingId,
                CriadorNome = t.CriadorNome,
                Nome = t.Nome,
                Descricao = t.Descricao,
                DistanciaKm = t.DistanciaKm,
                Dificuldade = t.Dificuldade,
                AvaliacaoMedia = t.AvaliacaoMedia,
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                CriadoEm = t.CriadoEm,
                ConcluidaPeloUsuario = concluida,
                Pontos = t.Pontos
                    .OrderBy(p => p.Ordem)
                    .Select(p => new TrilhaPontoResponse
                    {
                        Id = p.Id,
                        Ordem = p.Ordem,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude
                    }).ToList()
            };

        private static decimal CalcularDistanciaKm(List<TrilhaPontoRequest> pontos)
        {
            double total = 0;
            for (int i = 0; i < pontos.Count - 1; i++)
            {
                total += HaversineMetros(
                    (double)pontos[i].Latitude, (double)pontos[i].Longitude,
                    (double)pontos[i + 1].Latitude, (double)pontos[i + 1].Longitude);
            }
            return (decimal)(total / 1000.0);
        }

        private static double HaversineMetros(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000;
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }
}
