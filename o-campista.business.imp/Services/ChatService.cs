using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services
{
    public class ChatService : IChatService
    {
        private readonly IMensagemChatRepository _mensagemRepository;
        private readonly ICheckinRepository _checkinRepository;

        public ChatService(
            IMensagemChatRepository mensagemRepository,
            ICheckinRepository checkinRepository)
        {
            _mensagemRepository = mensagemRepository;
            _checkinRepository = checkinRepository;
        }

        public async Task<bool> TemCheckinValidoAsync(Guid usuarioId, long campingId)
        {
            return await _checkinRepository.TemCheckinNasUltimas24hAsync(usuarioId, campingId);
        }

        public async Task<MensagemChat> SalvarMensagemAsync(Guid usuarioId, long campingId, string texto)
        {
            var mensagem = new MensagemChat
            {
                UsuarioId = usuarioId,
                CampingId = campingId,
                Texto = texto,
                DataEnvio = DateTime.UtcNow
            };

            return await _mensagemRepository.SalvarAsync(mensagem);
        }

        public async Task<List<MensagemChatResponse>> ObterHistoricoAsync(long campingId, int limite, DateTime? antes)
        {
            var mensagens = await _mensagemRepository.ObterHistoricoAsync(campingId, limite, antes);

            return mensagens.Select(m => new MensagemChatResponse
            {
                Id = m.Id,
                CampingId = m.CampingId,
                UsuarioId = m.UsuarioId,
                NomeUsuario = m.Usuario.Nome,
                FotoUsuario = m.Usuario.FotoPerfil,
                Texto = m.Texto,
                DataEnvio = m.DataEnvio
            }).ToList();
        }
    }
}
