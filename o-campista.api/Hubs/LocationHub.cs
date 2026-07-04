using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

namespace o_campista.api.Hubs
{
    [Authorize]
    public class LocationHub : Hub
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISocialRepository _socialRepository;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan LocationTtl = TimeSpan.FromMinutes(5);
        private const string CacheKeyPrefix = "location:";

        public LocationHub(
            IUsuarioRepository usuarioRepository,
            ISocialRepository socialRepository,
            IMemoryCache cache)
        {
            _usuarioRepository = usuarioRepository;
            _socialRepository = socialRepository;
            _cache = cache;
        }

        public override async Task OnConnectedAsync()
        {
            var email = Context.User?.Identity?.Name;
            if (email is null) { Context.Abort(); return; }

            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario is null) { Context.Abort(); return; }

            // Verificar se o usuário tem visibilidade no mapa ativa
            var privacidade = await _socialRepository.ObterPrivacidadeAsync(usuario.Id);
            if (privacidade is null || !privacidade.VisivelNoMapa) { Context.Abort(); return; }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{usuario.Id}");
            await base.OnConnectedAsync();

            // Enviar localizações atuais dos seguidores mútuos já conectados
            var mutuos = await _socialRepository.ObterSeguidoresMutuosAsync(usuario.Id);
            foreach (var mutuo in mutuos)
            {
                if (_cache.TryGetValue<LocalizacaoUsuarioResponse>($"{CacheKeyPrefix}{mutuo.Id}", out var loc) && loc is not null)
                {
                    await Clients.Caller.SendAsync("ReceberLocalizacao", loc);
                }
            }
        }

        public async Task AtualizarLocalizacao(double lat, double lng)
        {
            var email = Context.User?.Identity?.Name;
            if (email is null) return;

            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario is null) return;

            var response = new LocalizacaoUsuarioResponse
            {
                UsuarioId = usuario.Id.ToString(),
                Nome = usuario.Nome,
                FotoUrl = usuario.FotoPerfil,
                Lat = lat,
                Lng = lng
            };

            // Armazenar com sliding expiration de 5 minutos
            _cache.Set($"{CacheKeyPrefix}{usuario.Id}", response, new MemoryCacheEntryOptions
            {
                SlidingExpiration = LocationTtl
            });

            // Enviar para os seguidores mútuos que estão conectados
            var mutuos = await _socialRepository.ObterSeguidoresMutuosAsync(usuario.Id);
            foreach (var mutuo in mutuos)
            {
                await Clients.Group($"user-{mutuo.Id}").SendAsync("ReceberLocalizacao", response);
            }
        }

        public async Task DesativarLocalizacao()
        {
            var email = Context.User?.Identity?.Name;
            if (email is null) return;

            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario is null) return;

            _cache.Remove($"{CacheKeyPrefix}{usuario.Id}");

            var mutuos = await _socialRepository.ObterSeguidoresMutuosAsync(usuario.Id);
            foreach (var mutuo in mutuos)
            {
                await Clients.Group($"user-{mutuo.Id}").SendAsync("RemoverLocalizacao", usuario.Id.ToString());
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var email = Context.User?.Identity?.Name;
            if (email is not null)
            {
                var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
                if (usuario is not null)
                {
                    _cache.Remove($"{CacheKeyPrefix}{usuario.Id}");

                    var mutuos = await _socialRepository.ObterSeguidoresMutuosAsync(usuario.Id);
                    foreach (var mutuo in mutuos)
                    {
                        await Clients.Group($"user-{mutuo.Id}").SendAsync("RemoverLocalizacao", usuario.Id.ToString());
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
