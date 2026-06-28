using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services
{
    public class SalaChatService : ISalaChatService
    {
        private readonly ISalaChatRepository _salaRepository;
        private readonly IMensagemSalaChatRepository _mensagemRepository;
        private readonly ICheckinRepository _checkinRepository;
        private readonly ICampingRepository _campingRepository;

        public SalaChatService(
            ISalaChatRepository salaRepository,
            IMensagemSalaChatRepository mensagemRepository,
            ICheckinRepository checkinRepository,
            ICampingRepository campingRepository)
        {
            _salaRepository = salaRepository;
            _mensagemRepository = mensagemRepository;
            _checkinRepository = checkinRepository;
            _campingRepository = campingRepository;
        }

        public async Task<List<SalaChatResponse>> ObterSalasAsync(Guid usuarioId)
        {
            var salas = await _salaRepository.ObterSalasDoUsuarioAsync(usuarioId);
            var naoLidas = await _mensagemRepository.ContarNaoLidasAsync(usuarioId);

            var responses = new List<SalaChatResponse>();

            foreach (var sala in salas)
            {
                var podeEnviar = true;
                if (sala.Tipo == "camping" && sala.CampingId.HasValue)
                    podeEnviar = await _checkinRepository.TemCheckinNasUltimas24hAsync(usuarioId, sala.CampingId.Value);

                var ultimaMensagem = sala.Mensagens.FirstOrDefault();

                responses.Add(new SalaChatResponse
                {
                    Id = sala.Id,
                    Nome = sala.Tipo == "camping" && sala.Camping is not null ? sala.Camping.Nome : sala.Nome,
                    Tipo = sala.Tipo,
                    CampingId = sala.CampingId,
                    FotoCapa = sala.FotoCapa,
                    CodigoConvite = sala.CodigoConvite,
                    DataCriacao = sala.CriadoEm,
                    TotalNaoLidas = naoLidas.TryGetValue(sala.Id, out var count) ? count : 0,
                    PodeEnviar = podeEnviar,
                    UltimaMensagem = ultimaMensagem is not null ? new UltimaMensagemResponse
                    {
                        Texto = ultimaMensagem.Texto,
                        NomeUsuario = ultimaMensagem.Usuario.Nome,
                        DataEnvio = ultimaMensagem.DataEnvio
                    } : null
                });
            }

            return responses;
        }

        public async Task<List<MensagemSalaChatResponse>> ObterMensagensAsync(long salaId, Guid usuarioId, int skip, int take)
        {
            var ehMembro = await _salaRepository.UsuarioEhMembroAsync(salaId, usuarioId);
            if (!ehMembro)
                throw new Exception("Você não é membro desta sala.");

            var mensagens = await _mensagemRepository.ObterHistoricoAsync(salaId, skip, take);

            return mensagens.Select(m => new MensagemSalaChatResponse
            {
                Id = m.Id,
                SalaId = m.SalaId,
                UsuarioId = m.UsuarioId,
                NomeUsuario = m.Usuario.Nome,
                FotoUsuario = m.Usuario.FotoPerfil,
                Texto = m.Texto,
                DataEnvio = m.DataEnvio
            }).ToList();
        }

        public async Task<CriarGrupoResponse> CriarGrupoAsync(Guid usuarioId, CriarGrupoRequest request)
        {
            var codigoConvite = GerarCodigoConvite();

            var sala = new SalaChat
            {
                Nome = request.Nome,
                Tipo = "grupo",
                CodigoConvite = codigoConvite,
                CriadoPorId = usuarioId,
                CriadoEm = DateTime.UtcNow
            };

            sala = await _salaRepository.CriarAsync(sala);

            await _salaRepository.AdicionarMembroAsync(new SalaChatMembro
            {
                SalaId = sala.Id,
                UsuarioId = usuarioId,
                EntradaEm = DateTime.UtcNow
            });

            return new CriarGrupoResponse
            {
                Sala = new SalaChatResponse
                {
                    Id = sala.Id,
                    Nome = sala.Nome,
                    Tipo = sala.Tipo,
                    CodigoConvite = sala.CodigoConvite,
                    DataCriacao = sala.CriadoEm,
                    PodeEnviar = true
                },
                CodigoConvite = codigoConvite
            };
        }

        public async Task<SalaChatResponse> EntrarGrupoAsync(Guid usuarioId, string codigoConvite)
        {
            var sala = await _salaRepository.ObterPorCodigoConviteAsync(codigoConvite);
            if (sala is null)
                throw new Exception("Código de convite inválido.");

            var jaMembro = await _salaRepository.UsuarioEhMembroAsync(sala.Id, usuarioId);
            if (jaMembro)
                throw new Exception("Você já é membro deste grupo.");

            await _salaRepository.AdicionarMembroAsync(new SalaChatMembro
            {
                SalaId = sala.Id,
                UsuarioId = usuarioId,
                EntradaEm = DateTime.UtcNow
            });

            return new SalaChatResponse
            {
                Id = sala.Id,
                Nome = sala.Nome,
                Tipo = sala.Tipo,
                CodigoConvite = sala.CodigoConvite,
                DataCriacao = sala.CriadoEm,
                PodeEnviar = true
            };
        }

        public async Task<string> ObterConviteAsync(long salaId, Guid usuarioId)
        {
            var sala = await _salaRepository.ObterPorIdAsync(salaId);
            if (sala is null)
                throw new Exception("Sala não encontrada.");

            var ehMembro = await _salaRepository.UsuarioEhMembroAsync(salaId, usuarioId);
            if (!ehMembro)
                throw new Exception("Você não é membro desta sala.");

            if (string.IsNullOrEmpty(sala.CodigoConvite))
                throw new Exception("Esta sala não possui código de convite.");

            return sala.CodigoConvite;
        }

        public async Task<List<MembroSalaResponse>> ObterMembrosAsync(long salaId)
        {
            var membros = await _salaRepository.ObterMembrosAsync(salaId);

            return membros.Select(m => new MembroSalaResponse
            {
                UsuarioId = m.UsuarioId,
                NomeUsuario = m.Usuario.Nome,
                FotoUsuario = m.Usuario.FotoPerfil
            }).ToList();
        }

        public async Task MarcarComoLidaAsync(long salaId, Guid usuarioId)
        {
            var ultimaMensagemId = await _mensagemRepository.ObterUltimaMensagemIdAsync(salaId);
            if (ultimaMensagemId.HasValue)
                await _mensagemRepository.AtualizarUltimaMensagemLidaAsync(salaId, usuarioId, ultimaMensagemId.Value);
        }

        public async Task<NaoLidasResponse> ObterNaoLidasAsync(Guid usuarioId)
        {
            var porSala = await _mensagemRepository.ContarNaoLidasAsync(usuarioId);

            return new NaoLidasResponse
            {
                Total = porSala.Values.Sum(),
                PorSala = porSala
            };
        }

        public async Task<PodeEnviarResponse> PodeEnviarAsync(long salaId, Guid usuarioId)
        {
            var sala = await _salaRepository.ObterPorIdAsync(salaId);
            if (sala is null)
                throw new Exception("Sala não encontrada.");

            if (sala.Tipo == "grupo")
                return new PodeEnviarResponse { PodeEnviar = true };

            if (!sala.CampingId.HasValue)
                return new PodeEnviarResponse { PodeEnviar = false };

            var podeEnviar = await _checkinRepository.TemCheckinNasUltimas24hAsync(usuarioId, sala.CampingId.Value);

            return new PodeEnviarResponse { PodeEnviar = podeEnviar };
        }

        public async Task<MensagemSalaChatResponse> SalvarMensagemAsync(long salaId, Guid usuarioId, string texto)
        {
            var mensagem = new MensagemSalaChat
            {
                SalaId = salaId,
                UsuarioId = usuarioId,
                Texto = texto,
                DataEnvio = DateTime.UtcNow
            };

            mensagem = await _mensagemRepository.SalvarAsync(mensagem);

            return new MensagemSalaChatResponse
            {
                Id = mensagem.Id,
                SalaId = mensagem.SalaId,
                UsuarioId = mensagem.UsuarioId,
                Texto = mensagem.Texto,
                DataEnvio = mensagem.DataEnvio
            };
        }

        public async Task<SalaChat> CriarSalaCampingSeNaoExisteAsync(long campingId, Guid usuarioId)
        {
            var salaExistente = await _salaRepository.ObterPorCampingIdAsync(campingId);

            if (salaExistente is not null)
            {
                var jaMembro = await _salaRepository.UsuarioEhMembroAsync(salaExistente.Id, usuarioId);
                if (!jaMembro)
                {
                    await _salaRepository.AdicionarMembroAsync(new SalaChatMembro
                    {
                        SalaId = salaExistente.Id,
                        UsuarioId = usuarioId,
                        EntradaEm = DateTime.UtcNow
                    });
                }
                return salaExistente;
            }

            var camping = await _campingRepository.ObterPorIdAsync(campingId);
            if (camping is null)
                throw new Exception("Camping não encontrado.");

            var sala = new SalaChat
            {
                Nome = camping.Nome,
                Tipo = "camping",
                CampingId = campingId,
                CriadoEm = DateTime.UtcNow
            };

            sala = await _salaRepository.CriarAsync(sala);

            await _salaRepository.AdicionarMembroAsync(new SalaChatMembro
            {
                SalaId = sala.Id,
                UsuarioId = usuarioId,
                EntradaEm = DateTime.UtcNow
            });

            return sala;
        }

        private static string GerarCodigoConvite()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }
    }
}
