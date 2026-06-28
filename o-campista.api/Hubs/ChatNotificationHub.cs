using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using o_campista.repository.IRepositories;

namespace o_campista.api.Hubs
{
    [Authorize]
    public class ChatNotificationHub : Hub
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ChatNotificationHub(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
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

            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{usuario.Id}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
