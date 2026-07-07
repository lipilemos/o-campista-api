using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using o_campista.business.imp.Dictionaries;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Enums;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IStorageService _storageService;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IStorageService storageService,
            ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<LoginResponse> ObterPerfilAsync(Guid usuarioId)
        {
            _logger.LogInformation(
                "Buscando perfil do usuário {UsuarioId}",
                usuarioId);

            var usuario =
                await _usuarioRepository
                    .ObterPorIdComDetalhesAsync(usuarioId);

            if (usuario is null)
            {
                _logger.LogWarning(
                    "Usuário {UsuarioId} não encontrado",
                    usuarioId);

                throw new Exception(
                    "Usuário não encontrado");
            }

            _logger.LogInformation(
                "Perfil encontrado para o usuário {UsuarioId}",
                usuarioId);

            return new LoginResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                FotoPerfil = usuario.FotoPerfil ?? string.Empty,
                Nivel = usuario.Nivel,
                Xp = usuario.XP,

                XpProximoNivel =
                    NivelXpDictionary.ObterXpProximoNivel(
                        usuario.Nivel),

                TotalCheckins =
                    usuario.Checkins.Count(),

                TotalCampingsVisitados =
                    usuario.Checkins
                        .Select(x => x.CampingId)
                        .Distinct()
                        .Count(),

                TotalTrilhasConcluidas = usuario.UsuarioTrilhas.Count(),

                Conquistas =
                    usuario.UsuarioConquistas
                        .Select(c => new ConquistaResponse
                        {
                            Id = c.Conquista.Id,
                            Nome = c.Conquista.Nome,
                            Descricao = c.Conquista.Descricao,
                            Icone = c.Conquista.Icone,
                            DataConquista = c.CriadoEm
                        }),

                Presentes =
                    usuario.UsuarioPresentes
                        .Select(p => new PresenteResponse
                        {
                            Id = p.Presente.Id,
                            Nome = p.Presente.Nome,
                            Descricao = p.Presente.Descricao,
                            CodigoResgate = p.Presente.CodigoResgate,
                            FotoUrl = p.Presente.FotoUrl,
                            Utilizado = p.Utilizado
                        })
            };
        }
        public async Task AdicionarXPAsync(Guid usuarioId, int xpAdicionado)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);

            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            usuario.XP += xpAdicionado;

            ValidarSubidaNivel(usuario);

            await _usuarioRepository.AtualizarAsync(usuario);
        }


        public async Task<LoginResponse> AtualizarFotoPerfilAsync(Guid usuarioId, IFormFile foto)
        {
            _logger.LogInformation(
                "Atualizando foto de perfil do usuário {UsuarioId}",
                usuarioId);

            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);

            if (usuario is null)
                throw new Exception("Usuário não encontrado.");

            var urlFoto = await _storageService.UploadAsync(foto, BucketTypeEnum.BucketUser);

            usuario.FotoPerfil = urlFoto;

            await _usuarioRepository.AtualizarAsync(usuario);

            _logger.LogInformation(
                "Foto de perfil atualizada para o usuário {UsuarioId}",
                usuarioId);

            return await ObterPerfilAsync(usuarioId);
        }

        public async Task DeletarContaAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);

            if (usuario is null)
                throw new Exception("Usuário não encontrado.");

            usuario.Ativo = false;
            usuario.Email = $"deleted_{usuarioId}@removed.com";
            usuario.Nome = "Usuário removido";
            usuario.SenhaHash = string.Empty;
            usuario.FotoPerfil = null;

            await _usuarioRepository.AtualizarAsync(usuario);

            _logger.LogInformation(
                "Conta do usuário {UsuarioId} desativada (LGPD)",
                usuarioId);
        }

        private void ValidarSubidaNivel(Usuario usuario)
        {
            while (usuario.XP >= NivelXpDictionary.ObterXpProximoNivel(usuario.Nivel))
            {
                usuario.Nivel++;
            }
        }
    }
}