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
        private readonly ISalaChatService _salaChatService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISalaChatRepository _salaChatRepository;
        private readonly IMemoryCache _cache;
        private readonly ChatConnectionStore _connectionStore;
        private readonly IHubContext<ChatNotificationHub> _notificationHub;

        public ChatHub(
            IChatService chatService,
            ISalaChatService salaChatService,
            IUsuarioRepository usuarioRepository,
            ISalaChatRepository salaChatRepository,
            IMemoryCache cache,
            ChatConnectionStore connectionStore,
            IHubContext<ChatNotificationHub> notificationHub)
        {
            _chatService = chatService;
            _salaChatService = salaChatService;
            _usuarioRepository = usuarioRepository;
            _salaChatRepository = salaChatRepository;
            _cache = cache;
            _connectionStore = connectionStore;
            _notificationHub = notificationHub;
        }

        public override async Task OnConnectedAsync()
        {
            var email = Context.User?.Identity?.Name;
            if (email is null)
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

            var salaIdStr = Context.GetHttpContext()?.Request.Query["salaId"].ToString();
            var campingIdStr = Context.GetHttpContext()?.Request.Query["campingId"].ToString();

            if (long.TryParse(salaIdStr, out var salaId))
            {
                var ehMembro = await _salaChatRepository.UsuarioEhMembroAsync(salaId, usuario.Id);
                if (!ehMembro)
                {
                    Context.Abort();
                    return;
                }

                _connectionStore.Add(Context.ConnectionId, new ChatConnectionInfo(
                    usuario.Id, 0, usuario.Nome, usuario.FotoPerfil, salaId));

                await Groups.AddToGroupAsync(Context.ConnectionId, $"sala-{salaId}");
            }
            else if (long.TryParse(campingIdStr, out var campingId))
            {
                var temCheckin = await _chatService.TemCheckinValidoAsync(usuario.Id, campingId);
                if (!temCheckin)
                {
                    Context.Abort();
                    return;
                }

                _connectionStore.Add(Context.ConnectionId, new ChatConnectionInfo(
                    usuario.Id, campingId, usuario.Nome, usuario.FotoPerfil, null));

                await Groups.AddToGroupAsync(Context.ConnectionId, $"camping-{campingId}");
            }
            else
            {
                Context.Abort();
                return;
            }

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

            if (info.SalaId.HasValue)
            {
                var podeEnviar = await _salaChatService.PodeEnviarAsync(info.SalaId.Value, info.UsuarioId);
                if (!podeEnviar.PodeEnviar)
                    throw new HubException("Você não pode enviar mensagens nesta sala. Check-in expirado.");

                var msgResponse = await _salaChatService.SalvarMensagemAsync(info.SalaId.Value, info.UsuarioId, texto);
                msgResponse.NomeUsuario = info.NomeUsuario;
                msgResponse.FotoUsuario = info.FotoUsuario;

                await Clients.Group($"sala-{info.SalaId.Value}").SendAsync("ReceberMensagem", msgResponse);

                var membros = await _salaChatRepository.ObterMembrosAsync(info.SalaId.Value);
                foreach (var membro in membros.Where(m => m.UsuarioId != info.UsuarioId))
                {
                    await _notificationHub.Clients.Group($"user-{membro.UsuarioId}")
                        .SendAsync("NovaMensagem", info.SalaId.Value);
                }
            }
            else
            {
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
        }

        public async Task Digitando()
        {
            if (!_connectionStore.TryGet(Context.ConnectionId, out var info) || info is null)
                return;

            if (info.SalaId.HasValue)
            {
                await Clients.OthersInGroup($"sala-{info.SalaId.Value}")
                    .SendAsync("UsuarioDigitando", info.NomeUsuario);
            }
            else
            {
                await Clients.OthersInGroup($"camping-{info.CampingId}")
                    .SendAsync("UsuarioDigitando", info.NomeUsuario);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectionStore.TryGet(Context.ConnectionId, out var info) && info is not null)
            {
                if (info.SalaId.HasValue)
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"sala-{info.SalaId.Value}");
                else
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

    public record ChatConnectionInfo(Guid UsuarioId, long CampingId, string NomeUsuario, string? FotoUsuario, long? SalaId = null);

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
