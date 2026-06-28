using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;
using System.Collections.Concurrent;
using System.Net;

namespace o_campista.api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMemoryCache _cache;
        private readonly ChatConnectionStore _connectionStore;

        public ChatHub(
            IChatService chatService,
            IUsuarioRepository usuarioRepository,
            IMemoryCache cache,
            ChatConnectionStore connectionStore)
        {
            _chatService = chatService;
            _usuarioRepository = usuarioRepository;
            _cache = cache;
            _connectionStore = connectionStore;
        }

        public override async Task OnConnectedAsync()
        {
            var email = Context.User?.Identity?.Name;
            if (email is null)
            {
                Context.Abort();
                return;
            }

            var campingIdStr = Context.GetHttpContext()?.Request.Query["campingId"].ToString();
            if (!long.TryParse(campingIdStr, out var campingId))
            {
                Context.Abort();
                return;
            }

            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario is null)
            {
                Context.Abort();
                return;
            }

            var temCheckin = await _chatService.TemCheckinValidoAsync(usuario.Id, campingId);
            if (!temCheckin)
            {
                Context.Abort();
                return;
            }

            _connectionStore.Add(Context.ConnectionId, new ChatConnectionInfo(
                usuario.Id, campingId, usuario.Nome, usuario.FotoPerfil));

            await Groups.AddToGroupAsync(Context.ConnectionId, $"camping-{campingId}");
            await base.OnConnectedAsync();
        }

        public async Task EnviarMensagem(string texto)
        {
            if (!_connectionStore.TryGet(Context.ConnectionId, out var info) || info is null)
                throw new HubException("Conexão inválida.");

            if (string.IsNullOrWhiteSpace(texto) || texto.Length > 500)
                throw new HubException("Texto deve ter entre 1 e 500 caracteres.");

            if (IsRateLimited(info.UsuarioId))
                throw new HubException("Limite de mensagens atingido. Aguarde um momento.");

            texto = WebUtility.HtmlEncode(texto.Trim());

            var mensagem = await _chatService.SalvarMensagemAsync(info.UsuarioId, info.CampingId, texto);

            var response = new MensagemChatResponse
            {
                Id = mensagem.Id,
                CampingId = mensagem.CampingId,
                UsuarioId = mensagem.UsuarioId,
                NomeUsuario = info.NomeUsuario,
                FotoUsuario = info.FotoUsuario,
                Texto = mensagem.Texto,
                DataEnvio = mensagem.DataEnvio
            };

            await Clients.Group($"camping-{info.CampingId}").SendAsync("ReceberMensagem", response);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectionStore.TryGet(Context.ConnectionId, out var info) && info is not null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"camping-{info.CampingId}");
                _connectionStore.Remove(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private bool IsRateLimited(Guid usuarioId)
        {
            var key = $"rl:chat:{usuarioId}";
            var entry = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return new RateLimitEntry();
            })!;

            if (entry.Count >= 10) return true;
            entry.Count++;
            return false;
        }

        private sealed class RateLimitEntry
        {
            public int Count { get; set; }
        }
    }

    public record ChatConnectionInfo(Guid UsuarioId, long CampingId, string NomeUsuario, string? FotoUsuario);

    public class ChatConnectionStore
    {
        private readonly ConcurrentDictionary<string, ChatConnectionInfo> _connections = new();

        public void Add(string connectionId, ChatConnectionInfo info) =>
            _connections[connectionId] = info;

        public bool TryGet(string connectionId, out ChatConnectionInfo? info) =>
            _connections.TryGetValue(connectionId, out info);

        public void Remove(string connectionId) =>
            _connections.TryRemove(connectionId, out _);
    }
}
